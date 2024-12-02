using Inventory.Domain.Abstractions;
using Inventory.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Transactions;

public class Transaction : AggregateRoot
{
    public Guid CreatorId { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public DateTime? CancelDate { get; private set; }
    public CostValue TotalCost { set; private get; }

    private List<TransactionItem> _items;
    public TransactionStatus Status { get; private set; }
    public ICollection<TransactionItem> Items { 
        get {
            return _items;
        } 
    }

    public Transaction(Guid creatorId) : base(Guid.NewGuid())
    {
        CreatorId = creatorId;
        Status = TransactionStatus.Created;
        CreationDate = DateTime.Now;
        TotalCost = 0;
        _items = new List<TransactionItem>();
    }

    public void AddItem(Guid itemId, int quantity, decimal unitaryCost)
    {
        if(Status != TransactionStatus.Created)
        {
            throw new InvalidOperationException("Cannot add items to a transaction that is not in Created status");
        }
        TransactionItem item = new TransactionItem(itemId, quantity, unitaryCost);
        _items.Add(item);
        TotalCost += item.SubTotal;
    }

    public void Complete()
    {
        if (Status != TransactionStatus.Created)
        {
            throw new InvalidOperationException("Cannot complete a transaction that is not in Created status");
        }
        if(_items.Count == 0)
        {
            throw new InvalidOperationException("Cannot complete a transaction with no items");
        }
        Status = TransactionStatus.Completed;
        CompletedDate = DateTime.Now;
    }

    public void Cancel()
    {
        if (Status != TransactionStatus.Created)
        {
            throw new InvalidOperationException("Cannot cancel a transaction that is not in Created status");
        }
        Status = TransactionStatus.Canceled;
        CancelDate = DateTime.Now;
    }

    public void UpdateItem(Guid itemId, int quantity, decimal unitaryCost)
    {
        if (Status != TransactionStatus.Created)
        {
            throw new InvalidOperationException("Cannot update items in a transaction that is not in Created status");
        }
        TransactionItem item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item == null)
        {
            throw new InvalidOperationException("Item not found in transaction");
        }
        TotalCost -= item.SubTotal;
        item.Update(quantity, unitaryCost);
        TotalCost += item.SubTotal;
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != TransactionStatus.Created)
        {
            throw new InvalidOperationException("Cannot remove items from a transaction that is not in Created status");
        }
        TransactionItem item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item == null)
        {
            throw new InvalidOperationException("Item not found in transaction");
        }
        TotalCost -= item.SubTotal;
        _items.Remove(item);
    }
}
