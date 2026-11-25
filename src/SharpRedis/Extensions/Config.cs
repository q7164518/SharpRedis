#pragma warning disable IDE1006
using SharpRedis.Enums;

namespace SharpRedis.Extensions;

internal static class Config
{
    internal const string LIB_NAME = "SharpRedis";

    internal const string UNKNOWN_ERROR_MSG = "Unknown error, error message cannot be obtained";

    /// <summary>
    /// \r
    /// </summary>
    internal const byte CR = 13;

    /// <summary>
    /// \n
    /// </summary>
    internal const byte LF = 10;

    internal const byte BROKEN = 0;

    #region RESP data type header
    /// <summary>
    /// $, Bulk strings
    /// </summary>
    internal const byte BULK_STRINGS = 36;

    /// <summary>
    /// *, Arrays
    /// </summary>
    internal const byte ARRAY = 42;

    /// <summary>
    /// > Pushes
    /// </summary>
    internal const byte PUSHES = 62;

    /// <summary>
    /// ~ Sets
    /// </summary>
    internal const byte SETS = 126;

    /// <summary>
    /// ! Bulk errors
    /// </summary>
    internal const byte BULK_ERRORS = 33;

    /// <summary>
    /// # boolean
    /// </summary>
    internal const byte BOOLEAN = 35;

    /// <summary>
    /// % Maps
    /// </summary>
    internal const byte MAPS = 37;

    /// <summary>
    /// + Simple strings
    /// </summary>
    internal const byte SIMPLE_STRINGS = 43;

    /// <summary>
    /// ( Big numbers
    /// </summary>
    internal const byte BIG_NUMBERS = 40;

    /// <summary>
    /// , double
    /// </summary>
    internal const byte DOUBLE = 44;

    /// <summary>
    /// : integer
    /// </summary>
    internal const byte INTEGER = 58;

    /// <summary>
    /// - Simple Errors
    /// </summary>
    internal const byte SIMPLE_ERRORS = 45;

    /// <summary>
    /// _ null
    /// </summary>
    internal const byte NULL = 95;

    /// <summary>
    /// = Verbatim strings
    /// </summary>
    internal const byte VERBATIM_STRINGS = 61;
    #endregion

    internal const byte O = 79;

    internal const byte o = 111;

    internal const byte K = 75;

    internal const byte k = 107;

    internal const byte T = 84;

    internal const byte t = 116;

    internal const byte ONE = 49;

    internal const string DEFAULT_HOST = "127.0.0.1";

    internal const int DEFAULT_PORT = 6379;

    internal const int DEFAULT_DATA_BASE = 0;

    internal const string DEFAULT_USER = "default";

    internal const int DEFAULT_BUFFER_SIZE = 4096;

    internal const int DEFAULT_BATCH_COUNT = 80;

    internal const int DEFAULT_HEARTBEAT_INTERVAL = 60_000;

    internal const int DEFAULT_COMMAND_TIMEOUT = 60_000;

    internal const int DEFAULT_RECONNECTION_INTERVAL = 5_000;

    internal const string CLIENT_CACHING_SUB_CHANNEL = "__redis__:invalidate";

    internal const uint CLUSTER_SLOTS = 16_384;

    internal const CommandMode WITH_CLIENT_CACHING_MODE = CommandMode.WithRead | CommandMode.WithClientCaching;
}
