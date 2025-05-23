﻿using Inventory.Infrastructure.Persistence.StoredModel.Entities;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.EFCore.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistence.StoredModel
{
    internal class StoredDbContext : DbContext, IDatabase
    {
        public DbSet<ItemStoredModel> Item { get; set; }
        public DbSet<TransactionStoredModel> Transaction { get; set; }
        public DbSet<TransactionItemStoredModel> TransactionItem { get; set; }
        public DbSet<UserStoredModel> User { get; set; }

        public StoredDbContext(DbContextOptions<StoredDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddOutboxModel<DomainEvent>();
        }

        public void Migrate()
        {
            Database.Migrate();
        }
    }
}
