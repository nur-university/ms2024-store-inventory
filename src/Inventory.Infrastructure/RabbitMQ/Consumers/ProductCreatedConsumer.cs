using Inventory.Application.Abstractions;
using Inventory.Application.Items.CreateItem;
using Joseco.Communication.External.RabbitMQ.Services;
using MediatR;
using Nur.Store2025.Integration.Catalog;

namespace Inventory.Infrastructure.RabbitMQ.Consumers;

public class ProductCreatedConsumer(IMediator mediator, ICorrelationIdProvider correlation) 
    : IIntegrationMessageConsumer<ProductCreated>
{
    public async Task HandleAsync(ProductCreated message, CancellationToken cancellationToken)
    {
        correlation.SetCorrelationId(message.CorrelationId!);

        CreateItemCommand command = new(
            message.ProductId,
            message.Name
        );

        await mediator.Send(command, cancellationToken);
    }
}
