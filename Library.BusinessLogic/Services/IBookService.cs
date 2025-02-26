using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookModel>> GetAllBooks();
        Task<BookModel> GetBookById(int id);
        Task<BookModel> GetBookByISBN(string isbn);
        Task AddBook(BookModel bookModel);
        Task UpdateBook(BookModel bookModel);
        Task DeleteBook(int id);
        Task BorrowBook(int bookId, DateTime dueDate);
        Task ReturnBook(int bookId);
        Task AddBookImage(int bookId, string imageUrl);
    }
}
