using Library.DataAccess.Context;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Security.Cryptography;
using System.Text;

namespace Library.Tests
{
    public class BookRepositoryTests
    {
        private readonly LibraryContext _context;
        private readonly IBookRepository _bookRepository;
        private readonly CancellationToken _cancellationToken;

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

            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task GetAllBooks_ReturnsAllBooks()
        {
            // Arrange & Act
            var result = await _bookRepository.GetAllBooks(_cancellationToken);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetBookById_ExistingId_ReturnsBook()
        {
            // Arrange & Act
            var result = await _bookRepository.GetBookById(1, _cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Book 1");
        }

        [Fact]
        public async Task GetBookById_NonExistingId_ReturnsNull()
        {
            // Arrange & Act
            var result = await _bookRepository.GetBookById(999, _cancellationToken);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBookByISBN_ExistingISBN_ReturnsBook()
        {
            // Arrange & Act
            var result = await _bookRepository.GetBookByISBN("1234567890123", _cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Book 1");
        }

        [Fact]
        public async Task GetBookByISBN_NonExistingISBN_ReturnsNull()
        {
            // Arrange & Act
            var result = await _bookRepository.GetBookByISBN("978-3-16-148410-0", _cancellationToken);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddBook_ValidBook_AddsBook()
        {
            // Arrange
            var book = new Book { Title = "New Book", ISBN = "1234567890125", Genre = "Science", Description = "Description 3", AuthorId = 1 };

            // Act
            await _bookRepository.AddBook(book, _cancellationToken);

            // Assert
            var addedBook = await _bookRepository.GetBookById(book.Id, _cancellationToken);
            addedBook.Should().NotBeNull();
            addedBook.Title.Should().Be("New Book");
            addedBook.ISBN.Should().Be("1234567890125");
        }

        [Fact]
        public async Task UpdateBook_ValidBook_UpdatesBook()
        {
            // Arrange
            var book = await _bookRepository.GetBookById(1, _cancellationToken);
            book.Title = "Updated Book";
            book.Description = "Updated Description";

            // Act
            await _bookRepository.UpdateBook(book, _cancellationToken);

            // Assert
            var updatedBook = await _bookRepository.GetBookById(1, _cancellationToken);
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
            await _bookRepository.DeleteBook(bookId, _cancellationToken);

            // Assert
            var deletedBook = await _bookRepository.GetBookById(bookId, _cancellationToken);
            deletedBook.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBook_NonExistingId_DoesNothing()
        {
            // Arrange
            var bookId = 999;

            // Act
            await _bookRepository.DeleteBook(bookId, _cancellationToken);

            // Assert
            var books = await _bookRepository.GetAllBooks(_cancellationToken);
            books.Should().HaveCount(2);
        }
    }
}
