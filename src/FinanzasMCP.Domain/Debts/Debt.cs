using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Debts;

public sealed class Debt : SoftDeletableEntity
{
    public DebtType Type { get; private set; }
    public string ContactName { get; private set; } = string.Empty;
    public decimal OriginalAmount { get; private set; }
    public decimal RemainingAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public DateTimeOffset? DueDate { get; private set; }
    public Guid? AccountId { get; private set; }
    public DebtStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public Account? Account { get; private set; }

    public ICollection<DebtPayment> Payments { get; private set; } = new List<DebtPayment>();

    public static Debt Create(DebtType type, string contactName, decimal originalAmount, decimal remainingAmount, string currency = "USD", DateTimeOffset? dueDate = null, Guid? accountId = null, string? notes = null)
        => new()
        {
            Type = type,
            ContactName = contactName.Trim(),
            OriginalAmount = originalAmount,
            RemainingAmount = remainingAmount,
            Currency = currency.Trim().ToUpperInvariant(),
            DueDate = dueDate,
            AccountId = accountId,
            Status = DebtStatus.Active,
            Notes = notes?.Trim()
        };

    public void RegisterPayment(decimal amount)
    {
        RemainingAmount -= amount;
        if (RemainingAmount <= 0) Status = DebtStatus.Paid;
        MarkUpdated();
    }

    public void UpdateDetails(DebtType type, string contactName, decimal originalAmount, decimal remainingAmount, string currency = "USD", DateTimeOffset? dueDate = null, Guid? accountId = null, DebtStatus? status = null, string? notes = null)
    {
        Type = type;
        ContactName = contactName.Trim();
        OriginalAmount = originalAmount;
        RemainingAmount = remainingAmount;
        Currency = currency.Trim().ToUpperInvariant();
        DueDate = dueDate;
        AccountId = accountId;
        if (status is not null)
        {
            Status = status.Value;
        }
        Notes = notes?.Trim();
        MarkUpdated();
    }
}
