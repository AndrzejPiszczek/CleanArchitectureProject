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
    public class BooksControllerTests
    {
        private readonly YourDbContext _context;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            var options = new DbContextOptionsBuilder<YourDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new YourDbContext(options);

            // Seed the in-memory database with test data
            _context.Books.AddRange(new List<Book>
            {
                new Book { Id = 1, Title = "Test Book 1", Genre = "Genre 1", AuthorId = 1 },
                new Book { Id = 2, Title = "Test Book 2", Genre = "Genre 2", AuthorId = 2 }
            });
            _context.SaveChanges();

            _controller = new BooksController(_context);
        }

        [Fact]
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Act
            var result = await _controller.GetBooks();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var books = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task GetBook_ReturnsBook()
        {
            // Act
            var result = await _controller.GetBook(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var book = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(1, book.Id);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetBook(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostBook_CreatesBook()
        {
            // Arrange
            var newBook = new Book { Title = "New Book", Genre = "New Genre", AuthorId = 3 };

            // Act
            var result = await _controller.PostBook(newBook);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var book = Assert.IsType<Book>(createdAtActionResult.Value);
            Assert.Equal("New Book", book.Title);
        }

        [Fact]
        public async Task PutBook_UpdatesBook()
        {
            // Arrange
            var updatedBook = new Book { Id = 1, Title = "Updated Book", Genre = "Updated Genre", AuthorId = 1 };

            // Act
            var result = await _controller.PutBook(1, updatedBook);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var book = await _context.Books.FindAsync(1);
            Assert.Equal("Updated Book", book.Title);
        }

        [Fact]
        public async Task DeleteBook_RemovesBook()
        {
            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var book = await _context.Books.FindAsync(1);
            Assert.Null(book);
        }
    }
}
