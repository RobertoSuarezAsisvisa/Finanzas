using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Debts;

public sealed class DebtInstallment : UserOwnedEntity
{
    public Guid DebtId { get; private set; }
    public int Number { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public decimal ExpectedPayment { get; private set; }
    public decimal Principal { get; private set; }
    public decimal Interest { get; private set; }
    public decimal BalanceAfterPayment { get; private set; }

    public Debt Debt { get; private set; } = null!;

    public static DebtInstallment Create(Guid debtId, int number, DateTimeOffset dueDate, decimal expectedPayment, decimal principal, decimal interest, decimal balanceAfterPayment)
        => new()
        {
            DebtId = debtId,
            Number = number,
            DueDate = dueDate,
            ExpectedPayment = expectedPayment,
            Principal = principal,
            Interest = interest,
            BalanceAfterPayment = balanceAfterPayment
        };
}
