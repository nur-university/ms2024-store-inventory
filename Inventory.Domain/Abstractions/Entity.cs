using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    public Entity(Guid id)
    {
        if(id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(id));
        }
        Id = id;
    }
}
