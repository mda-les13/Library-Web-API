using Library.DataAccess.Entities;

namespace Library.DataAccess.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthors(CancellationToken cancellationToken = default);
        Task<Author> GetAuthorById(int id, CancellationToken cancellationToken = default);
        Task AddAuthor(Author author, CancellationToken cancellationToken = default);
        Task UpdateAuthor(Author author, CancellationToken cancellationToken = default);
        Task DeleteAuthor(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Book>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken = default);
    }
}
