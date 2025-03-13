using Library.DataAccess.Entities;

namespace Library.DataAccess.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks(CancellationToken cancellationToken = default);
        Task<Book> GetBookById(int id, CancellationToken cancellationToken = default);
        Task<Book> GetBookByISBN(string isbn, CancellationToken cancellationToken = default);
        Task AddBook(Book book, CancellationToken cancellationToken = default);
        Task UpdateBook(Book book, CancellationToken cancellationToken = default);
        Task DeleteBook(int id, CancellationToken cancellationToken = default);
    }
}
