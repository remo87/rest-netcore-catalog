using Catalog.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Repositories
{
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Item> items = new()
        {
            new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreateDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreateDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreateDate = DateTimeOffset.UtcNow }
        };

        public IEnumerable<Item> GetItems()
        {
            return items;
        }

        public Item GetItem(Guid id)
        {
            return items.Where(item => item.Id == id).SingleOrDefault();
        }

        public void CreateItem(Item item)
        {
            items.Add(item);
        }

        public void UpdateItem(Item item)
        {
            var index = items.FindIndex(extItem => extItem.Id == item.Id);
            items[index] = item;
        }

        public void DeleteItem(Guid id)
        {
            var index = items.FindIndex(extItem => extItem.Id == id);
            items.RemoveAt(index);
        }
    }
}
