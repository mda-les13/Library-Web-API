using AutoMapper;
using Library.BusinessLogic.Services;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.BusinessLogic.Mappings;
using Library.BusinessLogic.Models;
using Moq;
using FluentAssertions;
using FluentValidation;

namespace Library.Tests
{
    public class BookServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IValidator<BookModel>> _mockBookValidator;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();

            _mockBookRepository = new Mock<IBookRepository>();
            _mockBookValidator = new Mock<IValidator<BookModel>>();

            _mockBookValidator.Setup(v => v.Validate(It.IsAny<BookModel>()))
                              .Returns((BookModel dto) => new FluentValidation.Results.ValidationResult());

            _bookService = new BookService(_mockBookRepository.Object, _mapper, _mockBookValidator.Object);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", ISBN = "1234567890123", Genre = "Fiction", Description = "Description 1", AuthorId = 1 },
                new Book { Id = 2, Title = "Book 2", ISBN = "1234567890124", Genre = "Non-Fiction", Description = "Description 2", AuthorId = 1 }
            };

            _mockBookRepository.Setup(repo => repo.GetAllBooks()).ReturnsAsync(books);

            // Act
            var result = await _bookService.GetAllBooks();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(books, options => options.Excluding(b => b.Id));
        }

        [Fact]
        public async Task GetBookById_ExistingId_ReturnsBook()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Book 1", ISBN = "1234567890123", Genre = "Fiction", Description = "Description 1", AuthorId = 1 };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);

            // Act
            var result = await _bookService.GetBookById(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(book, options => options.Excluding(b => b.Id));
        }

        [Fact]
        public async Task GetBookById_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookById(999)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.GetBookById(999));
        }

        [Fact]
        public async Task GetBookByISBN_ExistingISBN_ReturnsBook()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Book 1", ISBN = "1234567890123", Genre = "Fiction", Description = "Description 1", AuthorId = 1 };

            _mockBookRepository.Setup(repo => repo.GetBookByISBN("1234567890123")).ReturnsAsync(book);

            // Act
            var result = await _bookService.GetBookByISBN("1234567890123");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(book, options => options.Excluding(b => b.Id));
        }

        [Fact]
        public async Task GetBookByISBN_NonExistingISBN_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookByISBN("978-3-16-148410-0")).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.GetBookByISBN("978-3-16-148410-0"));
        }

        [Fact]
        public async Task AddBook_ValidBook_AddsBook()
        {
            // Arrange
            var bookDto = new BookModel
            {
                Title = "New Book",
                ISBN = "1234567890125",
                Genre = "Science",
                Description = "Description 3",
                AuthorId = 1
            };

            _mockBookRepository.Setup(repo => repo.AddBook(It.IsAny<Book>())).Verifiable();
            _mockBookRepository.Setup(repo => repo.GetBookByISBN("1234567890125")).ReturnsAsync((Book)null);

            // Act
            await _bookService.AddBook(bookDto);

            // Assert
            _mockBookRepository.Verify(repo => repo.AddBook(It.Is<Book>(b =>
                b.Title == bookDto.Title &&
                b.ISBN == bookDto.ISBN &&
                b.Genre == bookDto.Genre &&
                b.Description == bookDto.Description &&
                b.AuthorId == bookDto.AuthorId)), Times.Once);
        }

        [Fact]
        public async Task AddBook_DuplicateISBN_ThrowsArgumentException()
        {
            // Arrange
            var bookDto = new BookModel
            {
                Title = "New Book",
                ISBN = "1234567890123", // Existing ISBN
                Genre = "Science",
                Description = "Description 3",
                AuthorId = 1
            };

            var existingBook = new Book { Id = 1, Title = "Book 1", ISBN = "1234567890123", Genre = "Fiction", Description = "Description 1", AuthorId = 1 };

            _mockBookRepository.Setup(repo => repo.GetBookByISBN("1234567890123")).ReturnsAsync(existingBook);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.AddBook(bookDto));
        }

        [Fact]
        public async Task UpdateBook_ValidBook_UpdatesBook()
        {
            // Arrange
            var bookDto = new BookModel
            {
                Id = 1,
                Title = "Updated Book",
                ISBN = "1234567890123",
                Genre = "Fiction",
                Description = "Updated Description",
                AuthorId = 1
            };

            var existingBook = new Book { Id = 1, Title = "Old Book", ISBN = "1234567890123", Genre = "Fiction", Description = "Old Description", AuthorId = 1 };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(existingBook);
            _mockBookRepository.Setup(repo => repo.UpdateBook(It.IsAny<Book>())).Verifiable();

            // Act
            await _bookService.UpdateBook(bookDto);

            // Assert
            _mockBookRepository.Verify(repo => repo.UpdateBook(It.Is<Book>(b =>
                b.Id == bookDto.Id &&
                b.Title == bookDto.Title &&
                b.ISBN == bookDto.ISBN &&
                b.Genre == bookDto.Genre &&
                b.Description == bookDto.Description &&
                b.AuthorId == bookDto.AuthorId)), Times.Once);
        }

        [Fact]
        public async Task UpdateBook_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var bookDto = new BookModel
            {
                Id = 999,
                Title = "Updated Book",
                ISBN = "1234567890123",
                Genre = "Fiction",
                Description = "Updated Description",
                AuthorId = 1
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(999)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.UpdateBook(bookDto));
        }

        [Fact]
        public async Task UpdateBook_DuplicateISBN_ThrowsArgumentException()
        {
            // Arrange
            var bookDto = new BookModel
            {
                Id = 1,
                Title = "Updated Book",
                ISBN = "1234567890124", // Existing ISBN of another book
                Genre = "Fiction",
                Description = "Updated Description",
                AuthorId = 1
            };

            var existingBook = new Book { Id = 1, Title = "Old Book", ISBN = "1234567890123", Genre = "Fiction", Description = "Old Description", AuthorId = 1 };
            var anotherBook = new Book { Id = 2, Title = "Another Book", ISBN = "1234567890124", Genre = "Non-Fiction", Description = "Another Description", AuthorId = 1 };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(existingBook);
            _mockBookRepository.Setup(repo => repo.GetBookByISBN("1234567890124")).ReturnsAsync(anotherBook);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.UpdateBook(bookDto));
        }

        [Fact]
        public async Task DeleteBook_ExistingId_DeletesBook()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Book 1", ISBN = "1234567890123", Genre = "Fiction", Description = "Description 1", AuthorId = 1 };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);
            _mockBookRepository.Setup(repo => repo.DeleteBook(1)).Verifiable();

            // Act
            await _bookService.DeleteBook(1);

            // Assert
            _mockBookRepository.Verify(repo => repo.DeleteBook(1), Times.Once);
        }

        [Fact]
        public async Task DeleteBook_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookById(999)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.DeleteBook(999));
        }

        [Fact]
        public async Task BorrowBook_ValidBook_UpdatesBorrowedAndDueDates()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Book 1",
                ISBN = "1234567890123",
                Genre = "Fiction",
                Description = "Description 1",
                AuthorId = 1,
                BorrowedDate = null,
                DueDate = null
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);
            _mockBookRepository.Setup(repo => repo.UpdateBook(It.IsAny<Book>())).Verifiable();

            var dueDate = DateTime.Now.AddDays(7);

            // Act
            await _bookService.BorrowBook(1, dueDate);

            // Assert
            _mockBookRepository.Verify(repo => repo.UpdateBook(It.Is<Book>(b =>
                b.BorrowedDate.HasValue &&
                b.BorrowedDate.Value.Date == DateTime.Now.Date &&
                b.DueDate.HasValue &&
                b.DueDate.Value.Date == dueDate.Date)), Times.Once);
        }

        [Fact]
        public async Task BorrowBook_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookById(999)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.BorrowBook(999, DateTime.Now.AddDays(7)));
        }

        [Fact]
        public async Task ReturnBook_ValidBook_ClearsBorrowedAndDueDates()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Book 1",
                ISBN = "1234567890123",
                Genre = "Fiction",
                Description = "Description 1",
                AuthorId = 1,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7)
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);
            _mockBookRepository.Setup(repo => repo.UpdateBook(It.IsAny<Book>())).Verifiable();

            // Act
            await _bookService.ReturnBook(1);

            // Assert
            _mockBookRepository.Verify(repo => repo.UpdateBook(It.Is<Book>(b =>
                !b.BorrowedDate.HasValue &&
                !b.DueDate.HasValue)), Times.Once);
        }

        [Fact]
        public async Task ReturnBook_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookById(999)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.ReturnBook(999));
        }

        [Fact]
        public async Task AddBookImage_ValidBook_UpdatesImageUrl()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Book 1",
                ISBN = "1234567890123",
                Genre = "Fiction",
                Description = "Description 1",
                AuthorId = 1,
                ImageUrl = null
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);
            _mockBookRepository.Setup(repo => repo.UpdateBook(It.IsAny<Book>())).Verifiable();

            var imageUrl = "http://example.com/image.jpg";

            // Act
            await _bookService.AddBookImage(1, imageUrl);

            // Assert
            _mockBookRepository.Verify(repo => repo.UpdateBook(It.Is<Book>(b =>
                b.ImageUrl == imageUrl)), Times.Once);
        }

        [Fact]
        public async Task AddBookImage_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookById(999)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.AddBookImage(999, "http://example.com/image.jpg"));
        }
    }
}
