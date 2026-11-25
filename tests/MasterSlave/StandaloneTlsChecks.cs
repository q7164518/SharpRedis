using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using SharpRedis;
using SharpRedis.Exceptions;

internal static class StandaloneTlsChecks
{
    internal static async Task Run()
    {
        const string connectionString = "host=127.0.0.1,port=16688,ssl=true,sslHost=localhost";
        using var ca = X509Certificate2.CreateFromPem(File.ReadAllText("ssl/ca.pem"));
        using var client = X509CertificateLoader.LoadPkcs12FromFile(
            "ssl/client.pfx", "sharpredis-ssl-test", X509KeyStorageFlags.DefaultKeySet);

        bool ValidateServer(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
            => Validate(ca, certificate, chain, errors);

        using (var systemTrust = new X509Chain())
        {
            systemTrust.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            if (!systemTrust.Build(ca))
                Reject(() => Redis.UseStandalone(connectionString, o => o.ClientCertificates.Add(client)), "untrusted CA");
        }
        Reject(() => Redis.UseStandalone(connectionString, o => o.CertificateValidation = ValidateServer), "missing client certificate");
        Reject(() => Redis.UseStandalone(connectionString, o =>
        {
            o.ClientCertificates.Add(client);
            o.CertificateValidation = ValidateServer;
            o.SslHost = "wrong.example.invalid";
        }), "hostname mismatch");

        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        {
            using var redis = Redis.UseStandalone(connectionString, o =>
            {
                o.RespVersion = resp;
                o.CertificateValidation = ValidateServer;
                if (resp == Resp.RESP2) o.ClientCertificates.Add(client);
                else o.CertificateSelection = (_, _, _, _, _) => client;
            });
            string key = "tls-check:" + Guid.NewGuid().ToString("N");
            var large = new string('a', 512 * 1024);
            await redis.String.SetAsync(key, large, TimeSpan.FromMinutes(2));
            Check((string?)await redis.String.GetAsync(key) == large, "large TLS reply");
            using var pipe = redis.StartPipelining();
            await pipe.String.SetAsync(key, "pipeline", TimeSpan.FromMinutes(2));
            await pipe.String.GetAsync(key);
            Check((string?)(await pipe.ExecutePipeliningAsync())![1] == "pipeline", "TLS pipeline");

            using var db = redis.Select(1);
            Check((string?)await db.String.GetAsync(key) is null, "TLS database switch");
            await db.String.SetAsync(key, "db1", TimeSpan.FromMinutes(2));
            Check((string?)await db.String.GetAsync(key) == "db1", "TLS selected database");

            var message = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            await redis.PubSub.SubscribeAsync(key, (_, value) =>
            {
                message.TrySetResult(value.ToString()!);
                return Task.CompletedTask;
            });
            await redis.PubSub.PublishAsync(key, "tls");
            Check(await message.Task.WaitAsync(TimeSpan.FromSeconds(5)) == "tls", "TLS Pub/Sub");
            Console.WriteLine($"PASS {resp}: mutual TLS, large reply, pipeline, database switch and Pub/Sub");
        }
    }

    internal static bool Validate(X509Certificate2 ca, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
    {
        if (certificate is null || chain is null || (errors & ~SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            return false;
        using var leaf = X509CertificateLoader.LoadCertificate(certificate.GetRawCertData());
        using var trusted = new X509Chain();
        trusted.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        trusted.ChainPolicy.CustomTrustStore.Add(ca);
        trusted.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        trusted.ChainPolicy.ApplicationPolicy.Add(new("1.3.6.1.5.5.7.3.1"));
        return trusted.Build(leaf);
    }

    private static void Reject(Func<Redis> connect, string reason)
    {
        try
        {
            using var redis = connect();
        }
        catch (Exception ex) when (ex is AuthenticationException or IOException or RedisInitializationException
            || ex is NotSupportedException { Message: "The current connection is unavailable." })
        {
            Console.WriteLine($"PASS rejected {reason}");
            return;
        }
        throw new InvalidOperationException($"Unexpected TLS connection success: {reason}");
    }

    private static void Check(bool condition, string check)
    {
        if (!condition) throw new InvalidOperationException(check);
    }
}
