namespace IOBox.SqlServer;

internal static class MessageStatus
{
    public const byte New = 1;

    public const byte Locked = 2;

    public const byte Processed = 3;

    public const byte Failed = 4;

    public const byte Expired = 5;
}
