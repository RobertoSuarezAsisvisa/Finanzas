using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Domain.Shopping;

public sealed class ShoppingList : UserOwnedEntity
{
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset ListDate { get; private set; }
    public Guid? TransactionId { get; private set; }
    public Transaction? Transaction { get; private set; }
    public ICollection<ShoppingListItem> Items { get; private set; } = new List<ShoppingListItem>();

    public static ShoppingList Create(string name, DateTimeOffset listDate, Guid? transactionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Shopping list name is required.");
        }

        return new ShoppingList
        {
            Name = name.Trim(),
            ListDate = listDate,
            TransactionId = transactionId
        };
    }

    public void UpdateDetails(string name, DateTimeOffset listDate, Guid? transactionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Shopping list name is required.");
        }

        Name = name.Trim();
        ListDate = listDate;
        TransactionId = transactionId;
        MarkUpdated();
    }
}
