namespace SharpRedis.Enums;

internal enum ExpirationValueKind : byte
{
    TimeSpan = 0,

    DateTime = 1,

    Number = 2,
}
