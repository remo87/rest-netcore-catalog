using Catalog.Api;
using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new Mock<IItemsRepository>();
        private readonly Random rand = new Random();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            //arrange
            var expectedItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            //act
            var controller = new ItemsController(repositoryStub.Object);

            var actionResult = await controller.GetItemAsync(Guid.NewGuid());
            var result = actionResult.Result as OkObjectResult;
            //assert
            result.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItem_ReturnsAllItem()
        {
            //arrange
            var expetedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expetedItems);
            var controller = new ItemsController(repositoryStub.Object);

            //act
            var actualItems = await controller.GetItemsAsync();

            //assert
            actualItems.Should().BeEquivalentTo(expetedItems);
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            //arrange
            var itemToCrate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), rand.Next(1000));

            var controller = new ItemsController(repositoryStub.Object);

            //act
            var actionResult = await controller.CreateItem(itemToCrate);

            //assert
            var createdItem = (actionResult.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCrate.Should().BeEquivalentTo(
                createdItem,
                opt => opt.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreateDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingIteme_ReturnsNoContent()
        {
            //arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 10);

            var controller = new ItemsController(repositoryStub.Object);

            //act
            var result = await controller.UpdateItem(itemId, itemToUpdate);

            //assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingIteme_ReturnsNoContent()
        {
            //arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var controller = new ItemsController(repositoryStub.Object);

            //act
            var result = await controller.DeleteItem(existingItem.Id);

            //assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreateDate = DateTimeOffset.UtcNow
            };
        }
    }
}
