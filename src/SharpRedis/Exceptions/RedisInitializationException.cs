using System;

namespace SharpRedis.Exceptions;

public sealed class RedisInitializationException(string msg) : Exception(msg);
