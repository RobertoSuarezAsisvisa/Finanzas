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
    public decimal? InterestRate { get; private set; }
    public InterestPeriod? InterestPeriod { get; private set; }
    public AmortizationMethod? AmortizationMethod { get; private set; }
    public int? TermMonths { get; private set; }
    public DateTimeOffset? LoanStartDate { get; private set; }
    public Account? Account { get; private set; }

    public ICollection<DebtPayment> Payments { get; private set; } = new List<DebtPayment>();
    public ICollection<DebtInstallment> Installments { get; private set; } = new List<DebtInstallment>();

    public static Debt Create(DebtType type, string contactName, decimal originalAmount, decimal remainingAmount, string currency = "USD", DateTimeOffset? dueDate = null, Guid? accountId = null, string? notes = null, decimal? interestRate = null, InterestPeriod? interestPeriod = null, AmortizationMethod? amortizationMethod = null, int? termMonths = null, DateTimeOffset? loanStartDate = null)
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
            Notes = notes?.Trim(),
            InterestRate = interestRate,
            InterestPeriod = interestPeriod,
            AmortizationMethod = amortizationMethod,
            TermMonths = termMonths,
            LoanStartDate = loanStartDate
        };

    public void RegisterPayment(decimal amount)
    {
        AdjustPayment(amount);
    }

    public void AdjustPayment(decimal delta)
    {
        RemainingAmount -= delta;
        if (RemainingAmount <= 0)
        {
            RemainingAmount = 0m;
            Status = DebtStatus.Paid;
        }
        else if (Status == DebtStatus.Paid)
        {
            Status = DebtStatus.Active;
        }
        MarkUpdated();
    }

    public void UpdateDetails(DebtType type, string contactName, decimal originalAmount, decimal remainingAmount, string currency = "USD", DateTimeOffset? dueDate = null, Guid? accountId = null, DebtStatus? status = null, string? notes = null, decimal? interestRate = null, InterestPeriod? interestPeriod = null, AmortizationMethod? amortizationMethod = null, int? termMonths = null, DateTimeOffset? loanStartDate = null)
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
        InterestRate = interestRate;
        InterestPeriod = interestPeriod;
        AmortizationMethod = amortizationMethod;
        TermMonths = termMonths;
        LoanStartDate = loanStartDate;
        MarkUpdated();
    }
}
