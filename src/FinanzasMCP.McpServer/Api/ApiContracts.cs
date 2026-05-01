using FinanzasMCP.Domain.AccountingPeriods;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Budgets;
using FinanzasMCP.Domain.Categories;
using FinanzasMCP.Domain.Crypto;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Recurring;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.McpServer.Api;

public sealed record CreateAccountRequest(
    string Name,
    AccountType AccountType,
    string Currency,
    decimal Balance,
    string? BankName,
    string? AccountNumber,
    string? Provider,
    string? CryptoSymbol,
    string? CryptoNetwork,
    decimal? CryptoQuantity,
    decimal? CryptoAvgBuyPriceUsd);

public sealed record UpdateAccountRequest(
    string Name,
    string Currency,
    string? BankName,
    string? AccountNumber,
    string? Provider,
    bool IsActive);

public sealed record CreateCategoryRequest(string Name, CategoryType Type, string? Icon, Guid? ParentId, bool IsSystem);
public sealed record UpdateCategoryRequest(string Name, CategoryType Type, string? Icon, Guid? ParentId);

public sealed record CreateTagRequest(string Name, string? Color);
public sealed record UpdateTagRequest(string Name, string? Color);

public sealed record CreateTransactionRequest(
    TransactionType Type,
    decimal Amount,
    string Currency,
    Guid AccountId,
    Guid? ToAccountId,
    Guid? CategoryId,
    string? Description,
    string? Reference,
    DateTimeOffset TransactionDate,
    Guid? RecurringRuleId,
    IReadOnlyList<Guid>? TagIds);

public sealed record UpdateTransactionRequest(
    TransactionType Type,
    decimal Amount,
    string Currency,
    Guid AccountId,
    Guid? ToAccountId,
    Guid? CategoryId,
    string? Description,
    string? Reference,
    DateTimeOffset TransactionDate,
    Guid? RecurringRuleId,
    IReadOnlyList<Guid>? TagIds);

public sealed record CreateBudgetRequest(string Name, Guid CategoryId, decimal LimitAmount, PeriodType PeriodType, BudgetValidityType ValidityType, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd);
public sealed record UpdateBudgetRequest(string Name, decimal LimitAmount, PeriodType PeriodType, BudgetValidityType ValidityType, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd, bool IsActive);

public sealed record CreateCryptoAccountRequest(Guid AccountId, string Symbol, string? Network, decimal Quantity, decimal? AvgBuyPriceUsd);
public sealed record UpdateCryptoAccountRequest(Guid AccountId, string Symbol, string? Network, decimal Quantity, decimal? AvgBuyPriceUsd);

public sealed record CreateCryptoLotRequest(Guid AccountId, decimal Quantity, decimal BuyPriceUsd, CryptoLotStatus Status, Guid? TransactionId, decimal? SellPriceUsd, DateTimeOffset? OperationDate);
public sealed record UpdateCryptoLotRequest(Guid AccountId, decimal Quantity, decimal BuyPriceUsd, CryptoLotStatus Status, Guid? TransactionId, decimal? SellPriceUsd, DateTimeOffset? OperationDate);

public sealed record CreateAccountingPeriodRequest(string Name, DateTimeOffset StartDate, DateTimeOffset EndDate);
public sealed record UpdateAccountingPeriodRequest(string Name, DateTimeOffset StartDate, DateTimeOffset EndDate, AccountingPeriodStatus Status, decimal TotalIncome, decimal TotalExpenses, decimal NetBalance, DateTimeOffset? ClosedAt);

public sealed record CreateSavingGoalRequest(string Name, decimal TargetAmount, Guid? AccountId, DateTimeOffset? TargetDate);
public sealed record UpdateSavingGoalRequest(string Name, decimal TargetAmount, Guid? AccountId, DateTimeOffset? TargetDate, SavingGoalStatus? Status);
public sealed record AddSavingGoalContributionRequest(decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);
public sealed record UpdateSavingGoalContributionRequest(decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);

public sealed record CreatePurchaseGoalRequest(string Name, decimal TargetPrice, string? Description, int Priority, string? Url, Guid? AccountId, DateTimeOffset? TargetDate);
public sealed record UpdatePurchaseGoalRequest(string Name, decimal TargetPrice, string? Description, int Priority, string? Url, Guid? AccountId, DateTimeOffset? TargetDate, PurchaseGoalStatus? Status, DateTimeOffset? PurchasedAt);
public sealed record AddPurchaseGoalContributionRequest(decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);
public sealed record UpdatePurchaseGoalContributionRequest(decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);

public sealed record CreateDebtRequest(DebtType Type, string ContactName, decimal OriginalAmount, decimal RemainingAmount, string Currency, DateTimeOffset? DueDate, Guid? AccountId, string? Notes);
public sealed record UpdateDebtRequest(DebtType Type, string ContactName, decimal OriginalAmount, decimal RemainingAmount, string Currency, DateTimeOffset? DueDate, Guid? AccountId, DebtStatus? Status, string? Notes);
public sealed record RegisterDebtPaymentRequest(decimal Amount, DateTimeOffset PaymentDate, string? Notes, Guid? TransactionId);
public sealed record UpdateDebtPaymentRequest(decimal Amount, DateTimeOffset PaymentDate, string? Notes, Guid? TransactionId);

public sealed record CreateRecurringRuleRequest(string Name, RecurringType Type, decimal Amount, Guid AccountId, Guid? CategoryId, RecurringFrequency Frequency, DateTimeOffset StartDate, DateTimeOffset? EndDate, DateTimeOffset NextDueDate);
public sealed record UpdateRecurringRuleRequest(string Name, RecurringType Type, decimal Amount, Guid AccountId, Guid? CategoryId, RecurringFrequency Frequency, DateTimeOffset StartDate, DateTimeOffset? EndDate, DateTimeOffset NextDueDate, bool IsActive);

public sealed record UpsertUserContextEntryRequest(string Value);
