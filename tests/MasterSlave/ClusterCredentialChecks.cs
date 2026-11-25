using System.Reflection;
using SharpRedis;

internal static class ClusterCredentialChecks
{
    internal static void Run()
    {
        // Rejected registrations use an unreachable endpoint: validation must run before any connection attempt.
        Reject(() => Redis.UseCluster(false, new[]
        {
            "host=127.0.0.1,port=1,user=reader,password=credential-a",
            "host=127.0.0.1,port=1,user=reader,password=credential-b",
        }), "Nodes[1].Password");
        Reject(() => Redis.UseCluster(true, new[]
        {
            "host=127.0.0.1,port=1,user=reader,password=credential-a",
            "host=127.0.0.1,port=1,user=Reader,password=credential-a",
        }), "Nodes[1].User");
        Reject(() => Redis.UseCluster(false, o =>
        {
            o.Password = "credential-a";
            o.Nodes.Add(new() { Port = 1, Password = "credential-b" });
        }), "Nodes[0].Password");
        Reject(() => Redis.UseCluster(false, new[] { "host=127.0.0.1,port=1,password=credential-a" }, o =>
        {
            o.Password = "credential-b";
        }), "Nodes[0].Password");
        Reject(() => Redis.UseCluster(false, new[] { "host=127.0.0.1,port=1,password=credential-a" }, o =>
        {
            o.Nodes.Add(new() { Port = 1, Password = "credential-b" });
        }), "Nodes[1].Password");
        Reject(() => Redis.UseCluster(false, o => o.Nodes.Add(null!)), "Nodes[0]");

        var uniform = new ClusterConnectionOptions(new[]
        {
            "host=127.0.0.1,user=reader,password=credential-a",
            "host=127.0.0.1,user=reader,password=credential-a",
        });
        Validate(uniform);
        Check(uniform.User == "reader" && uniform.Password == "credential-a", "Uniform credentials changed");
        var inherited = new ClusterConnectionOptions { User = "reader", Password = "credential-a" };
        inherited.Nodes.Add(new()); inherited.Nodes.Add(new() { Password = "credential-a" });
        Validate(inherited);
        Check(inherited.User == "reader" && inherited.Password == "credential-a", "Inheritance failed");
        var inferred = new ClusterConnectionOptions();
        inferred.Nodes.Add(new()); inferred.Nodes.Add(new() { User = "reader", Password = "credential-a" });
        Validate(inferred);
        Check(inferred.User == "reader" && inferred.Password == "credential-a", "Explicit credentials ignored");
        var anonymous = new ClusterConnectionOptions(); anonymous.Nodes.Add(new()); Validate(anonymous);
        Check(anonymous.User is null && anonymous.Password is null, "Anonymous authentication changed");
        Console.WriteLine("PASS Cluster credentials: both registration overloads, callback changes, username/password conflicts, inheritance, inference and no secret disclosure");
    }
    private static void Validate(ClusterConnectionOptions options)
        => typeof(ClusterConnectionOptions).GetMethod("ValidateCredentials", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(options, null);
    private static void Reject(Func<Redis> action, string field)
    {
        try { using var redis = action(); throw new Exception("Conflicting credentials accepted"); }
        catch (ArgumentException ex)
        {
            Check(ex.Message.Contains(field), "Wrong conflict location");
            Check(!ex.Message.Contains("credential-a") && !ex.Message.Contains("credential-b"), "Password disclosed");
        }
    }
    private static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
}
