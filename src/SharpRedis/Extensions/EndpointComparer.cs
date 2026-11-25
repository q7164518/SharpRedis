using System;
using System.Collections.Generic;

namespace SharpRedis.Extensions;

internal sealed class EndpointComparer : IEqualityComparer<(string Host, int Port)>
{
    internal static readonly EndpointComparer _instance = new();

    private static StringComparer Comparer(string host) => BasalConnectionOptions.IsUnixSocketHost(host)
        ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

    public bool Equals((string Host, int Port) x, (string Host, int Port) y)
        => x.Port == y.Port && Comparer(x.Host).Equals(x.Host, y.Host);

    public int GetHashCode((string Host, int Port) endpoint)
        => unchecked(Comparer(endpoint.Host).GetHashCode(endpoint.Host) * 397 ^ endpoint.Port);
}
