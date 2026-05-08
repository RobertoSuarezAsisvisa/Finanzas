using FinanzasMCP.Domain.Debts;

namespace FinanzasMCP.Application.Debts.Services;

public sealed class DebtInstallmentScheduleService
{
    public IReadOnlyList<DebtInstallment> Build(Debt debt)
    {
        if (debt.TermMonths is null or <= 0 || debt.AmortizationMethod is null || debt.InterestPeriod is null)
        {
            return [];
        }

        var principal = debt.OriginalAmount;
        if (principal <= 0)
        {
            return [];
        }

        var termMonths = Math.Min(debt.TermMonths.Value, 480);
        var monthlyRate = ((debt.InterestRate ?? 0m) / 100m) / (debt.InterestPeriod == InterestPeriod.Annual ? 12m : 1m);
        var startDate = debt.LoanStartDate ?? debt.DueDate ?? DateTimeOffset.UtcNow;
        var balance = principal;
        var fixedPrincipal = principal / termMonths;
        var frenchPayment = monthlyRate > 0
            ? principal * (monthlyRate / (1 - (decimal)Math.Pow((double)(1 + monthlyRate), -termMonths)))
            : principal / termMonths;
        var rows = new List<DebtInstallment>(termMonths);

        for (var number = 1; number <= termMonths; number++)
        {
            var interest = balance * monthlyRate;
            var principalPayment = debt.AmortizationMethod == AmortizationMethod.German
                ? fixedPrincipal
                : frenchPayment - interest;
            var payment = debt.AmortizationMethod == AmortizationMethod.German
                ? principalPayment + interest
                : frenchPayment;

            balance = Math.Max(0m, balance - principalPayment);
            rows.Add(DebtInstallment.Create(
                debt.Id,
                number,
                startDate.AddMonths(number - 1),
                Round(payment),
                Round(principalPayment),
                Round(interest),
                Round(balance)));
        }

        return rows;
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
