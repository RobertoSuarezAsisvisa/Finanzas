using FinanzasMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.McpServer.Auth;

public sealed class LegacyDataClaimer(FinanzasMCPDbContext dbContext)
{
    private static readonly string[] Tables =
    [
        "accounts",
        "accounting_periods",
        "budgets",
        "categories",
        "crypto_accounts",
        "crypto_lots",
        "debt_installments",
        "debt_payments",
        "debts",
        "purchase_goal_contributions",
        "purchase_goals",
        "recurring_rules",
        "saving_goal_contributions",
        "saving_goals",
        "tags",
        "transaction_tags",
        "transactions",
        "user_context"
    ];

    public async Task ClaimForFirstUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        foreach (var table in Tables)
        {
            var sql = "UPDATE \"" + table + "\" SET \"UserId\" = @p0 WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000'";
            await dbContext.Database.ExecuteSqlRawAsync(
                sql,
                [userId],
                cancellationToken);
        }
    }
}
