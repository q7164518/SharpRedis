using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SharpRedis.Entities;

internal sealed class SharpConnectionOptions
{
#if MODERN_NET
    private readonly string? _user;
    private readonly string? _password;
    private readonly string? _connectName;
    private readonly object? _transportOptions;
#else
    private readonly string _user;
    private readonly string _password;
    private readonly string _connectName;
    private readonly TlsOptions _tls;
#endif
    private readonly string _host;
    private readonly int _port;
    private readonly Resp _resp;
    private readonly AddressFamily _addressFamily;
    private readonly int _batchCount;
    private readonly int _buffSize;
    private readonly int _database;
    private readonly int _commandTimeout;
    private readonly byte[] _keyPrefix;
    private readonly byte[] _keySuffix;
    private readonly Encoding _encoding;

    internal string Host => this._host;

    internal int Port => this._port;

    internal Resp RespVersion => this._resp;

#if MODERN_NET
    internal string? User => this._user;

    internal string? Password => this._password;

    internal string? ConnectName => this._connectName;

    internal TlsOptions? Tls => this._transportOptions as TlsOptions;

    internal UnixDomainSocketEndPoint? UnixEndpoint => this._transportOptions as UnixDomainSocketEndPoint;
#else
    internal string User => this._user;

    internal string Password => this._password;

    internal string ConnectName => this._connectName;

    internal TlsOptions Tls => this._tls;
#endif

    internal AddressFamily HostFamily => this._addressFamily;

    internal Encoding Encoding => this._encoding;

    internal int BatchCount => this._batchCount;

    internal int BuffSize => this._buffSize;

    internal int Database => this._database;

    internal int CommandTimeout => this._commandTimeout;

    internal byte[] KeyPrefix => this._keyPrefix;

    internal byte[] KeySuffix => this._keySuffix;

    internal SharpConnectionOptions(
        string host,
        int port,
        Resp resp,
#if MODERN_NET
        string? user,
        string? password,
        string? connectName,
        string? prefix,
        string? suffix,
#else
        string user,
        string password,
        string connectName,
        string prefix,
        string suffix,
#endif
        AddressFamily addressFamily,
        int batchCount,
        int buffSize,
        int database,
        int commandTimeout,
        Encoding encoding,
#if MODERN_NET
        TlsOptions? tls = null,
        string? unixSocketHost = null
#else
        TlsOptions tls = null
#endif
    )
    {
        this._host = host;
        this._port = port;
        this._resp = resp;
        this._user = user;
        this._password = password;
        this._connectName = connectName;
        this._addressFamily = addressFamily;
        this._batchCount = batchCount;
        this._buffSize = buffSize;
        this._database = database;
        this._commandTimeout = commandTimeout;
        this._encoding = encoding;
#if MODERN_NET
        this._transportOptions = tls;
        if (unixSocketHost is not null || addressFamily == AddressFamily.Unix)
        {
            var socketPath = unixSocketHost ?? host;
            if (!BasalConnectionOptions.IsUnixSocketHost(socketPath) || socketPath.Contains('\0'))
                throw new ArgumentException("Invalid Redis Unix socket path.", nameof(host));
            if (tls is not null)
                throw new ArgumentException("Redis Unix socket and TLS cannot be combined on the same endpoint.", nameof(host));
#if NET5_0_OR_GREATER
            if (!Socket.OSSupportsUnixDomainSockets)
                throw new PlatformNotSupportedException("The operating system does not support Unix domain sockets.");
#endif
            this._transportOptions = new UnixDomainSocketEndPoint(socketPath);
        }
#else
        this._tls = tls;
#endif
        if (string.IsNullOrEmpty(prefix))
        {
            this._keyPrefix = [];
        }
        else
        {
            this._keyPrefix = encoding.GetBytes(prefix);
        }
        if (string.IsNullOrEmpty(suffix))
        {
            this._keySuffix = [];
        }
        else
        {
            this._keySuffix = encoding.GetBytes(suffix);
        }
    }

    internal sealed class TlsOptions
    {
        internal bool CheckCertificateRevocation { get; }

        internal string Host { get; }

        internal X509CertificateCollection ClientCertificates { get; }

#if MODERN_NET
        internal RemoteCertificateValidationCallback? CertificateValidation { get; }

        internal LocalCertificateSelectionCallback? CertificateSelection { get; }
#else
        internal RemoteCertificateValidationCallback CertificateValidation { get; }

        internal LocalCertificateSelectionCallback CertificateSelection { get; }
#endif

#nullable enable
        internal static TlsOptions? Create(BasalConnectionOptions options, BasalConnectionOptions? endpoint = null, bool enable = false)
        {
            if (!enable && !options.Ssl && endpoint?.Ssl != true) return null;
            return new TlsOptions(options, endpoint);
        }

        private TlsOptions(BasalConnectionOptions options, BasalConnectionOptions? endpoint)
        {
            this.CheckCertificateRevocation = options.CheckCertificateRevocation || endpoint?.CheckCertificateRevocation == true;
            this.Host = endpoint?.SslHost ?? options.SslHost ?? string.Empty;
            var certificates = endpoint?.ConfiguredCertificates is { Count: > 0 } local
                ? local : options.ConfiguredCertificates;
            this.ClientCertificates = certificates is null ? new() : new(certificates);
            this.CertificateValidation = endpoint?.CertificateValidation ?? options.CertificateValidation;
            this.CertificateSelection = endpoint is not null && (endpoint.CertificateSelection is not null || endpoint.ConfiguredCertificates is { Count: > 0 })
                ? endpoint.CertificateSelection : options.CertificateSelection;
        }
#nullable restore
    }
}
