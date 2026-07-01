using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.CreditCards;

public sealed class CreditCardAccount : UserOwnedEntity
{
    public Guid AccountId { get; private set; }
    public string Issuer { get; private set; } = string.Empty;
    public CreditCardBrand Brand { get; private set; }
    public string? ProductName { get; private set; }
    public string? LastFour { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal OutstandingBalance { get; private set; }
    public decimal AvailableCredit { get; private set; }
    public int StatementClosingDay { get; private set; }
    public int PaymentDueDay { get; private set; }
    public CreditCardPaymentMode PaymentMode { get; private set; } = CreditCardPaymentMode.Manual;
    public string? RewardsProgram { get; private set; }
    public CreditCardStatementDelivery StatementDelivery { get; private set; } = CreditCardStatementDelivery.Virtual;
    public decimal? InterestNominalAnnual { get; private set; }
    public decimal? InterestEffectiveAnnual { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Account Account { get; private set; } = null!;
    public ICollection<CreditCardStatement> Statements { get; private set; } = new List<CreditCardStatement>();
    public ICollection<CreditCardTransaction> CreditCardTransactions { get; private set; } = new List<CreditCardTransaction>();

    public static CreditCardAccount Create(
        Guid accountId,
        string issuer,
        CreditCardBrand brand,
        decimal creditLimit,
        int statementClosingDay,
        int paymentDueDay,
        string? productName = null,
        string? lastFour = null,
        decimal outstandingBalance = 0m,
        CreditCardPaymentMode paymentMode = CreditCardPaymentMode.Manual,
        string? rewardsProgram = null,
        CreditCardStatementDelivery statementDelivery = CreditCardStatementDelivery.Virtual,
        decimal? interestNominalAnnual = null,
        decimal? interestEffectiveAnnual = null)
    {
        ValidateLimit(creditLimit);
        ValidateDay(statementClosingDay, nameof(statementClosingDay));
        ValidateDay(paymentDueDay, nameof(paymentDueDay));
        if (outstandingBalance < 0) throw new InvalidOperationException("Outstanding balance cannot be negative.");
        if (outstandingBalance > creditLimit) throw new InvalidOperationException("Outstanding balance cannot exceed credit limit.");

        return new CreditCardAccount
        {
            AccountId = accountId,
            Issuer = issuer.Trim(),
            Brand = brand,
            ProductName = productName?.Trim(),
            LastFour = NormalizeLastFour(lastFour),
            CreditLimit = creditLimit,
            OutstandingBalance = outstandingBalance,
            AvailableCredit = creditLimit - outstandingBalance,
            StatementClosingDay = statementClosingDay,
            PaymentDueDay = paymentDueDay,
            PaymentMode = paymentMode,
            RewardsProgram = rewardsProgram?.Trim(),
            StatementDelivery = statementDelivery,
            InterestNominalAnnual = interestNominalAnnual,
            InterestEffectiveAnnual = interestEffectiveAnnual,
            IsActive = true
        };
    }

    public void UpdateDetails(
        string issuer,
        CreditCardBrand brand,
        decimal creditLimit,
        int statementClosingDay,
        int paymentDueDay,
        string? productName,
        string? lastFour,
        CreditCardPaymentMode paymentMode,
        string? rewardsProgram,
        CreditCardStatementDelivery statementDelivery,
        decimal? interestNominalAnnual,
        decimal? interestEffectiveAnnual,
        bool isActive)
    {
        ValidateLimit(creditLimit);
        ValidateDay(statementClosingDay, nameof(statementClosingDay));
        ValidateDay(paymentDueDay, nameof(paymentDueDay));
        if (OutstandingBalance > creditLimit) throw new InvalidOperationException("Credit limit cannot be lower than the outstanding balance.");

        Issuer = issuer.Trim();
        Brand = brand;
        ProductName = productName?.Trim();
        LastFour = NormalizeLastFour(lastFour);
        CreditLimit = creditLimit;
        StatementClosingDay = statementClosingDay;
        PaymentDueDay = paymentDueDay;
        PaymentMode = paymentMode;
        RewardsProgram = rewardsProgram?.Trim();
        StatementDelivery = statementDelivery;
        InterestNominalAnnual = interestNominalAnnual;
        InterestEffectiveAnnual = interestEffectiveAnnual;
        IsActive = isActive;
        RecalculateAvailableCredit();
        MarkUpdated();
    }

    public void RegisterCharge(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("Amount must be positive.");
        if (OutstandingBalance + amount > CreditLimit) throw new InvalidOperationException("Credit card limit exceeded.");
        OutstandingBalance += amount;
        RecalculateAvailableCredit();
        MarkUpdated();
    }

    public void RegisterPayment(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("Amount must be positive.");
        OutstandingBalance = Math.Max(0m, OutstandingBalance - amount);
        RecalculateAvailableCredit();
        MarkUpdated();
    }

    private void RecalculateAvailableCredit()
        => AvailableCredit = Math.Max(0m, CreditLimit - OutstandingBalance);

    private static void ValidateLimit(decimal creditLimit)
    {
        if (creditLimit <= 0) throw new InvalidOperationException("Credit limit must be positive.");
    }

    private static void ValidateDay(int day, string fieldName)
    {
        if (day is < 1 or > 31) throw new InvalidOperationException($"{fieldName} must be between 1 and 31.");
    }

    private static string? NormalizeLastFour(string? lastFour)
    {
        var value = lastFour?.Trim();
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (value.Length > 4) throw new InvalidOperationException("Last four must have at most 4 characters.");
        return value;
    }
}
