using Library.DataAccess.Context;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Library.Tests
{
    public class BookRepositoryTests
    {
        private readonly LibraryContext _context;
        private readonly IBookRepository _bookRepository;

        public BookRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new LibraryContext(options);
            _bookRepository = new BookRepository(_context);

            // Seed initial data
            _context.Books.AddRange(
                new Book { Id = 1, Title = "Book 1", ISBN = "1234567890123", Genre = "Fiction", Description = "Description 1", AuthorId = 1 },
                new Book { Id = 2, Title = "Book 2", ISBN = "1234567890124", Genre = "Non-Fiction", Description = "Description 2", AuthorId = 1 }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllBooks_ReturnsAllBooks()
        {
            // Arrange & Act
            var result = await _bookRepository.GetAllBooks();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetBookById_ExistingId_ReturnsBook()
        {
            // Arrange & Act
            var result = await _bookRepository.GetBookById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Book 1");
        }

        [Fact]
        public async Task GetBookById_NonExistingId_ReturnsNull()
        {
            // Arrange & Act
            var result = await _bookRepository.GetBookById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddBook_ValidBook_AddsBook()
        {
            // Arrange
            var book = new Book { Title = "New Book", ISBN = "1234567890125", Genre = "Science", Description = "Description 3", AuthorId = 1 };

            // Act
            await _bookRepository.AddBook(book);

            // Assert
            var addedBook = await _bookRepository.GetBookById(book.Id);
            addedBook.Should().NotBeNull();
            addedBook.Title.Should().Be("New Book");
            addedBook.ISBN.Should().Be("1234567890125");
        }

        [Fact]
        public async Task UpdateBook_ValidBook_UpdatesBook()
        {
            // Arrange
            var book = await _bookRepository.GetBookById(1);
            book.Title = "Updated Book";
            book.Description = "Updated Description";

            // Act
            await _bookRepository.UpdateBook(book);

            // Assert
            var updatedBook = await _bookRepository.GetBookById(1);
            updatedBook.Should().NotBeNull();
            updatedBook.Title.Should().Be("Updated Book");
            updatedBook.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task DeleteBook_ExistingId_DeletesBook()
        {
            // Arrange
            var bookId = 1;

            // Act
            await _bookRepository.DeleteBook(bookId);

            // Assert
            var deletedBook = await _bookRepository.GetBookById(bookId);
            deletedBook.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBook_NonExistingId_DoesNothing()
        {
            // Arrange
            var bookId = 999;

            // Act
            await _bookRepository.DeleteBook(bookId);

            // Assert
            var books = await _bookRepository.GetAllBooks();
            books.Should().HaveCount(2);
        }
    }
}
