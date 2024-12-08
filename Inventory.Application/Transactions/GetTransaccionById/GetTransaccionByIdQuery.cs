﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Transactions.GetTransaccionById;

public record GetTransaccionByIdQuery(Guid TransactionId) : IRequest<TransactionDto>;