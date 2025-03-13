using AutoMapper;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.BusinessLogic.Models;
using FluentValidation;

namespace Library.BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<BookModel> _validator;

        public BookService(IBookRepository bookRepository, IMapper mapper, IValidator<BookModel> validator)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<BookModel>> GetAllBooks(CancellationToken cancellationToken = default)
        {
            var books = await _bookRepository.GetAllBooks(cancellationToken);
            return _mapper.Map<IEnumerable<BookModel>>(books);
        }

        public async Task<BookModel> GetBookById(int id, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetBookById(id, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }
            return _mapper.Map<BookModel>(book);
        }

        public async Task<BookModel> GetBookByISBN(string isbn, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetBookByISBN(isbn, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ISBN {isbn} not found.");
            }
            return _mapper.Map<BookModel>(book);
        }

        public async Task AddBook(BookModel bookDto, CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(bookDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingBook = await _bookRepository.GetBookByISBN(bookDto.ISBN, cancellationToken);
            if (existingBook != null)
            {
                throw new ArgumentException($"Book with ISBN {bookDto.ISBN} already exists.");
            }

            var book = _mapper.Map<Book>(bookDto);
            await _bookRepository.AddBook(book, cancellationToken);
        }

        public async Task UpdateBook(BookModel bookModel, CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(bookModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Check if book exists
            var existingBook = await _bookRepository.GetBookById(bookModel.Id, cancellationToken);
            if (existingBook == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookModel.Id} not found.");
            }

            if (existingBook.ISBN != bookModel.ISBN)
            {
                var bookWithSameISBN = await _bookRepository.GetBookByISBN(bookModel.ISBN, cancellationToken);
                if (bookWithSameISBN != null)
                {
                    throw new ArgumentException($"Book with ISBN {bookModel.ISBN} already exists.");
                }
            }

            var updatedBook = _mapper.Map<Book>(bookModel);
            await _bookRepository.UpdateBook(updatedBook, cancellationToken);
        }

        public async Task DeleteBook(int id, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetBookById(id, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }

            await _bookRepository.DeleteBook(id, cancellationToken);
        }

        public async Task BorrowBook(int bookId, DateTime dueDate, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetBookById(bookId, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }

            book.BorrowedDate = DateTime.Now;
            book.DueDate = dueDate;
            await _bookRepository.UpdateBook(book, cancellationToken);
        }

        public async Task ReturnBook(int bookId, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetBookById(bookId, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }

            book.BorrowedDate = null;
            book.DueDate = null;
            await _bookRepository.UpdateBook(book, cancellationToken);
        }

        public async Task AddBookImage(int bookId, string imageUrl, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetBookById(bookId, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }

            book.ImageUrl = imageUrl;
            await _bookRepository.UpdateBook(book, cancellationToken);
        }
    }
}
