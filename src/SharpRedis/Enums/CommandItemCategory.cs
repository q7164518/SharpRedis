namespace SharpRedis.Enums;

internal enum CommandItemCategory : byte
{
    None = 0,

    Command,

    Key,

    Argument,
}
