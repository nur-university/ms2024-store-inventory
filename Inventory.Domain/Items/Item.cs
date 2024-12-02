﻿using Inventory.Domain.Abstractions;
using Inventory.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Items;

public class Item : AggregateRoot
{

    public string Name { get; private set; }
    public int Stock { get; private set; }
    public CostValue UnitaryCost { get; private set; }

    public Item(Guid id, string name) : base(id)
    {
        Name = name;
        Stock = 0;
        UnitaryCost = 0;
    }

    public void UpdateStockAndCost(int quantityToAdd, decimal unitaryCost)
    {
        if(quantityToAdd > 0)
        {
            CostValue newCost = (UnitaryCost * Stock + unitaryCost * quantityToAdd) / (Stock + quantityToAdd);
            UnitaryCost = newCost;
        }
        Stock += quantityToAdd;
    }
}