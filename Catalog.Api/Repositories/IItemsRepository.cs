﻿using Catalog.Api.Entities;
using System;
using System.Collections.Generic;

namespace Catalog.Api.Repositories
{
    public interface IItemsRepository
    {
        Item GetItem(Guid id);
        IEnumerable<Item> GetItems();
        void CreateItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(Guid id);
    }
}