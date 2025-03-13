using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookModel>> GetAllBooks(CancellationToken cancellationToken = default);
        Task<BookModel> GetBookById(int id, CancellationToken cancellationToken = default);
        Task<BookModel> GetBookByISBN(string isbn, CancellationToken cancellationToken = default);
        Task AddBook(BookModel bookModel, CancellationToken cancellationToken = default);
        Task UpdateBook(BookModel bookModel, CancellationToken cancellationToken = default);
        Task DeleteBook(int id, CancellationToken cancellationToken = default);
        Task BorrowBook(int bookId, DateTime dueDate, CancellationToken cancellationToken = default);
        Task ReturnBook(int bookId, CancellationToken cancellationToken = default);
        Task AddBookImage(int bookId, string imageUrl, CancellationToken cancellationToken = default);
    }
}
