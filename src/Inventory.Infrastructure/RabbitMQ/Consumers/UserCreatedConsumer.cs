﻿using Inventory.Application.Users.CreateUser;
using Joseco.Communication.External.RabbitMQ.Services;
using MediatR;
using Nur.Store2025.Integration.Identity;

namespace Inventory.Infrastructure.RabbitMQ.Consumers;

class UserCreatedConsumer(IMediator mediator) : IIntegrationMessageConsumer<UserCreated>
{
    public async Task HandleAsync(UserCreated message, CancellationToken cancellationToken)
    {
        CreateUserCommand command = new(message.UserId, message.FullName);

        await mediator.Send(command, cancellationToken);
    }
}