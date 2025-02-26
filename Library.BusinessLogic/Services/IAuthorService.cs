using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorModel>> GetAllAuthors();
        Task<AuthorModel> GetAuthorById(int id);
        Task AddAuthor(AuthorModel authorModel);
        Task UpdateAuthor(AuthorModel authorModel);
        Task DeleteAuthor(int id);
        Task<IEnumerable<BookModel>> GetBooksByAuthor(int authorId);
    }
}
