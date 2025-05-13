﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Abstractions;

public interface ICorrelationIdProvider
{
    string GetCorrelationId();

    void SetCorrelationId(string correlationId);
}
