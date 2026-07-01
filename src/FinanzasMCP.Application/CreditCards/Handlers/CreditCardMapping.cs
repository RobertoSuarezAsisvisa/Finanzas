using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.CreditCards.Handlers;

internal static class CreditCardMapping
{
    public static CreditCardSummary ToSummary(CreditCardAccount creditCard)
    {
        var nextDueDate = creditCard.Statements
            .Where(x => x.Status is CreditCardStatementStatus.Closed or CreditCardStatementStatus.Overdue)
            .OrderBy(x => x.DueDate)
            .Select(x => (DateTimeOffset?)x.DueDate)
            .FirstOrDefault();

        return new CreditCardSummary(
            creditCard.Id,
            creditCard.AccountId,
            creditCard.Account.Name,
            creditCard.Account.Currency,
            creditCard.Issuer,
            creditCard.Brand,
            creditCard.ProductName,
            creditCard.LastFour,
            creditCard.CreditLimit,
            creditCard.OutstandingBalance,
            creditCard.AvailableCredit,
            creditCard.StatementClosingDay,
            creditCard.PaymentDueDay,
            creditCard.PaymentMode,
            creditCard.RewardsProgram,
            creditCard.StatementDelivery,
            creditCard.InterestNominalAnnual,
            creditCard.InterestEffectiveAnnual,
            creditCard.IsActive,
            nextDueDate);
    }

    public static AccountSummary ToAccountSummary(Account account)
    {
        var cryptoAccount = account.AccountType == AccountType.Crypto ? account.CryptoAccount : null;
        var creditCard = account.AccountType == AccountType.CreditCard ? account.CreditCardAccount : null;

        return new AccountSummary(
            account.Id,
            account.Name,
            account.AccountType,
            account.Currency,
            account.Purpose,
            account.Balance,
            account.IsActive,
            account.BankName,
            account.AccountNumber,
            account.Provider,
            cryptoAccount?.Symbol,
            cryptoAccount?.Network,
            cryptoAccount?.Quantity,
            cryptoAccount?.AvgBuyPriceUsd,
            creditCard?.Id,
            creditCard?.Issuer,
            creditCard?.Brand,
            creditCard?.ProductName,
            creditCard?.LastFour,
            creditCard?.CreditLimit,
            creditCard?.OutstandingBalance,
            creditCard?.AvailableCredit,
            creditCard?.StatementClosingDay,
            creditCard?.PaymentDueDay,
            creditCard?.PaymentMode,
            creditCard?.RewardsProgram,
            creditCard?.StatementDelivery,
            creditCard?.InterestNominalAnnual,
            creditCard?.InterestEffectiveAnnual);
    }

    public static CreditCardStatementSummary ToStatementSummary(CreditCardStatement statement)
        => new(
            statement.Id,
            statement.CreditCardAccountId,
            statement.PeriodStart,
            statement.PeriodEnd,
            statement.StatementDate,
            statement.DueDate,
            statement.OpeningBalance,
            statement.Purchases,
            statement.Fees,
            statement.Interest,
            statement.Payments,
            statement.StatementBalance,
            statement.MinimumPayment,
            statement.Status);
}
