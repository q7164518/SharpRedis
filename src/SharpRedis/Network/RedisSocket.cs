using SharpRedis.Entities;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SharpRedis.Network;

/// <summary>
/// The transport is the socket itself, so plain TCP needs no wrapper or TLS state.
/// </summary>
internal class RedisSocket : Socket
{
    protected RedisSocket(AddressFamily family, ProtocolType protocol)
        : base(family, SocketType.Stream, protocol) { }

    internal static RedisSocket Open(SharpConnectionOptions options, int timeout)
    {
#if MODERN_NET
        if (options.UnixEndpoint is { } endpoint)
            return UnixSocket.Open(endpoint, EffectiveTimeout(timeout, options.CommandTimeout));
#endif
        var tls = options.Tls;
        RedisSocket socket = tls is null
            ? new RedisSocket(options.HostFamily, ProtocolType.Tcp)
            : new TlsSocket(options.HostFamily);
        try
        {
            socket.NoDelay = true;
            if (tls is not null) timeout = EffectiveTimeout(timeout, options.CommandTimeout);
            if (timeout > 0) Wait(socket.ConnectAsync(options.Host, options.Port), timeout, "Redis connection timed out.");
            else socket.Connect(options.Host, options.Port);
            if (socket is TlsSocket secure) secure.Authenticate(tls!, options.Host, timeout);
            return socket;
        }
        catch
        {
            socket.CloseTransport();
            throw;
        }
    }

    private static int EffectiveTimeout(int timeout, int commandTimeout)
        => timeout > 0 ? timeout : commandTimeout > 0 ? commandTimeout : 5000;

    protected static void Wait(Task pending, int timeout, string message)
    {
        try
        {
            if (!pending.Wait(timeout)) throw new TimeoutException(message);
        }
        catch (AggregateException)
        {
            pending.GetAwaiter().GetResult();
            throw;
        }
    }

    internal virtual void SendBuffer(byte[] buffer, int length)
    {
        int sent = 0;
        while (sent < length)
        {
            int count = Send(buffer, sent, length - sent, SocketFlags.None);
            if (count <= 0) throw new IOException("Connection has been reset. Data has not been fully sent.");
            sent += count;
        }
    }

#if MODERN_NET
    internal virtual IAsyncResult StartRead(byte[] buffer, AsyncCallback callback, object? state)
#else
    internal virtual IAsyncResult StartRead(byte[] buffer, AsyncCallback callback, object state)
#endif
        => BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, callback, state);

    internal virtual int FinishRead(IAsyncResult result) => EndReceive(result);

    internal virtual void CloseTransport() => Dispose();

    private sealed class TlsSocket(AddressFamily family) : RedisSocket(family, ProtocolType.Tcp)
    {
        private SslStream _stream = null!;

        internal void Authenticate(SharpConnectionOptions.TlsOptions tls, string host, int timeout)
        {
            SendTimeout = timeout;
            _stream = new SslStream(new NetworkStream(this, ownsSocket: false), false,
                tls.CertificateValidation, tls.CertificateSelection);
            Wait(_stream.AuthenticateAsClientAsync(string.IsNullOrWhiteSpace(tls.Host) ? host : tls.Host,
                tls.ClientCertificates, SslProtocols.None, tls.CheckCertificateRevocation), timeout, "Redis TLS handshake timed out.");
        }

        internal override void SendBuffer(byte[] buffer, int length)
            => _stream.Write(buffer, 0, length);

#if MODERN_NET
        internal override IAsyncResult StartRead(byte[] buffer, AsyncCallback callback, object? state)
#else
        internal override IAsyncResult StartRead(byte[] buffer, AsyncCallback callback, object state)
#endif
            => _stream.BeginRead(buffer, 0, buffer.Length, callback, state);

        internal override int FinishRead(IAsyncResult result)
            => _stream.EndRead(result);

        internal override void CloseTransport()
        {
            Dispose();
            _stream?.Dispose();
        }
    }

#if MODERN_NET
    private sealed class UnixSocket() : RedisSocket(AddressFamily.Unix, ProtocolType.Unspecified)
    {
        internal static UnixSocket Open(UnixDomainSocketEndPoint endpoint, int timeout)
        {
            var socket = new UnixSocket();
            try
            {
                Wait(socket.ConnectAsync(endpoint), timeout, "Redis connection timed out.");
                return socket;
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }
    }
#endif
}
