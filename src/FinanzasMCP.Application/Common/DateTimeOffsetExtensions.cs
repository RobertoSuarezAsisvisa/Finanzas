namespace FinanzasMCP.Application.Common;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset ToUtcSafe(this DateTimeOffset value) => value.ToUniversalTime();
}
