using FinanzasMCP.Domain.Tags;

namespace FinanzasMCP.Domain.Transactions;

public sealed class TransactionTag
{
    public Guid TransactionId { get; private set; }
    public Guid TagId { get; private set; }

    public Tag Tag { get; private set; } = null!;
}
