using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorModel>> GetAllAuthors(CancellationToken cancellationToken = default);
        Task<AuthorModel> GetAuthorById(int id, CancellationToken cancellationToken = default);
        Task AddAuthor(AuthorModel authorModel, CancellationToken cancellationToken = default);
        Task UpdateAuthor(AuthorModel authorModel, CancellationToken cancellationToken = default);
        Task DeleteAuthor(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<BookModel>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken = default);
    }
}
