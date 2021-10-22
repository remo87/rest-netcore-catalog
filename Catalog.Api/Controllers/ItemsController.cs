using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<ItemDto> GetItems()
        {
            var items = repository.GetItems().Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id)
        {
            var item = repository.GetItem(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item.AsDto());
        }

        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto)
        {
            Item item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreateDate = DateTimeOffset.UtcNow
            };
            repository.CreateItem(item);

            return CreatedAtAction(nameof(GetItem), new { Id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = repository.GetItem(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            repository.UpdateItem(updatedItem);

            return NoContent();
        }

        [HttpDelete]
        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = repository.GetItem(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            repository.DeleteItem(id);

            return NoContent();
        }
    }
}
