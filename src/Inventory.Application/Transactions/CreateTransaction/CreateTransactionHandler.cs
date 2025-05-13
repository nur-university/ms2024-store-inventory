using Inventory.Domain.Items;
using Inventory.Domain.Transactions;
using Inventory.Domain.Transactions.Exceptions;
using MediatR;
using Joseco.DDD.Core.Abstractions;

namespace Inventory.Application.Transactions.CreateTransaction;

internal class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, Guid>
{
    private readonly ITransactionFactory _transactionFactory;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransactionHandler(ITransactionFactory transactionFactory,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _transactionFactory = transactionFactory;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {

        var transaction = request.Type switch
        {
            "Entry" => _transactionFactory.CreateEntryTransaction(request.UserCreatorId),
            "Exit" => _transactionFactory.CreateExitTransaction(request.UserCreatorId),
            _ => throw new TransactionCreationException("Invalid transaction type")
        };

        foreach (var item in request.Items)
        {
            var storeItem = await _itemRepository.GetByIdAsync(item.ItemId, true);
            if (storeItem == null)
            {
                throw new TransactionCreationException("Item not found");
            }

            transaction.AddItem(item.ItemId, item.Quantity, item.UnitaryCost);
        }

        await _transactionRepository.AddAsync(transaction);

        await _unitOfWork.CommitAsync(cancellationToken);

        return transaction.Id;
    }
}