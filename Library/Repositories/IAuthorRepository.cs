using Library.DataAccess.Entities;

namespace Library.DataAccess.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthors();
        Task<Author> GetAuthorById(int id);
        Task AddAuthor(Author author);
        Task UpdateAuthor(Author author);
        Task DeleteAuthor(int id);
        Task<IEnumerable<Book>> GetBooksByAuthor(int authorId);
    }
}
