using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.AccountingPeriods;

public sealed class AccountingPeriod : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public decimal TotalIncome { get; private set; }
    public decimal TotalExpenses { get; private set; }
    public decimal NetBalance { get; private set; }
    public AccountingPeriodStatus Status { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    public static AccountingPeriod Create(string name, DateTimeOffset startDate, DateTimeOffset endDate)
        => new()
        {
            Name = name.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            Status = AccountingPeriodStatus.Open
        };

    public void UpdateDetails(string name, DateTimeOffset startDate, DateTimeOffset endDate, AccountingPeriodStatus status, decimal totalIncome, decimal totalExpenses, decimal netBalance, DateTimeOffset? closedAt)
    {
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        TotalIncome = totalIncome;
        TotalExpenses = totalExpenses;
        NetBalance = netBalance;
        ClosedAt = closedAt;
        MarkUpdated();
    }

    public void Close(decimal totalIncome, decimal totalExpenses, decimal netBalance, DateTimeOffset closedAt)
    {
        TotalIncome = totalIncome;
        TotalExpenses = totalExpenses;
        NetBalance = netBalance;
        Status = AccountingPeriodStatus.Closed;
        ClosedAt = closedAt;
        MarkUpdated();
    }

    public void Reopen()
    {
        Status = AccountingPeriodStatus.Open;
        ClosedAt = null;
        MarkUpdated();
    }
}
