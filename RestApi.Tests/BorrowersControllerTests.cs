using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Controllers;
using RestApi.Data;
using RestApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RestApi.Tests
{
    public class BorrowersControllerTests
    {
        private readonly YourDbContext _context;
        private readonly BorrowersController _controller;

        public BorrowersControllerTests()
        {
            var options = new DbContextOptionsBuilder<YourDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new YourDbContext(options);

            _context.Borrowers.AddRange(new List<Borrower>
            {
                new Borrower { Id = 1, Name = "Borrower 1" },
                new Borrower { Id = 2, Name = "Borrower 2" }
            });
            _context.SaveChanges();

            _controller = new BorrowersController(_context);
        }

        [Fact]
        public async Task GetBorrowers_ReturnsAllBorrowers()
        {
            var result = await _controller.GetBorrowers();
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Borrower>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var borrowers = Assert.IsType<List<Borrower>>(okResult.Value);
            Assert.Equal(2, borrowers.Count);
        }

        [Fact]
        public async Task GetBorrower_ReturnsBorrower()
        {
            var result = await _controller.GetBorrower(1);
            var actionResult = Assert.IsType<ActionResult<Borrower>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var borrower = Assert.IsType<Borrower>(okResult.Value);
            Assert.Equal(1, borrower.Id);
        }

        [Fact]
        public async Task GetBorrower_ReturnsNotFound()
        {
            var result = await _controller.GetBorrower(99);
            var actionResult = Assert.IsType<ActionResult<Borrower>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostBorrower_CreatesBorrower()
        {
            var newBorrower = new Borrower { Name = "New Borrower" };
            var result = await _controller.PostBorrower(newBorrower);
            var actionResult = Assert.IsType<ActionResult<Borrower>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var borrower = Assert.IsType<Borrower>(createdAtActionResult.Value);
            Assert.Equal("New Borrower", borrower.Name);
        }

        [Fact]
        public async Task PutBorrower_UpdatesBorrower()
        {
            var updatedBorrower = new Borrower { Id = 1, Name = "Updated Borrower" };
            var result = await _controller.PutBorrower(1, updatedBorrower);
            Assert.IsType<NoContentResult>(result);
            var borrower = await _context.Borrowers.FindAsync(1);
            Assert.Equal("Updated Borrower", borrower.Name);
        }

        [Fact]
        public async Task DeleteBorrower_RemovesBorrower()
        {
            var result = await _controller.DeleteBorrower(1);
            Assert.IsType<NoContentResult>(result);
            var borrower = await _context.Borrowers.FindAsync(1);
            Assert.Null(borrower);
        }
    }
}
