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
    public class AuthorsControllerTests
    {
        private readonly YourDbContext _context;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            var options = new DbContextOptionsBuilder<YourDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new YourDbContext(options);

            _context.Authors.AddRange(new List<Author>
            {
                new Author { Id = 1, Name = "Author 1" },
                new Author { Id = 2, Name = "Author 2" }
            });
            _context.SaveChanges();

            _controller = new AuthorsController(_context);
        }

        [Fact]
        public async Task GetAuthors_ReturnsAllAuthors()
        {
            var result = await _controller.GetAuthors();
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Author>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var authors = Assert.IsType<List<Author>>(okResult.Value);
            Assert.Equal(2, authors.Count);
        }

        [Fact]
        public async Task GetAuthor_ReturnsAuthor()
        {
            var result = await _controller.GetAuthor(1);
            var actionResult = Assert.IsType<ActionResult<Author>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var author = Assert.IsType<Author>(okResult.Value);
            Assert.Equal(1, author.Id);
        }

        [Fact]
        public async Task GetAuthor_ReturnsNotFound()
        {
            var result = await _controller.GetAuthor(99);
            var actionResult = Assert.IsType<ActionResult<Author>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostAuthor_CreatesAuthor()
        {
            var newAuthor = new Author { Name = "New Author" };
            var result = await _controller.PostAuthor(newAuthor);
            var actionResult = Assert.IsType<ActionResult<Author>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var author = Assert.IsType<Author>(createdAtActionResult.Value);
            Assert.Equal("New Author", author.Name);
        }

        [Fact]
        public async Task PutAuthor_UpdatesAuthor()
        {
            var updatedAuthor = new Author { Id = 1, Name = "Updated Author" };
            var result = await _controller.PutAuthor(1, updatedAuthor);
            Assert.IsType<NoContentResult>(result);
            var author = await _context.Authors.FindAsync(1);
            Assert.Equal("Updated Author", author.Name);
        }

        [Fact]
        public async Task DeleteAuthor_RemovesAuthor()
        {
            var result = await _controller.DeleteAuthor(1);
            Assert.IsType<NoContentResult>(result);
            var author = await _context.Authors.FindAsync(1);
            Assert.Null(author);
        }
    }
}
