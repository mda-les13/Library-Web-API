using Library.DataAccess.Context;
using Library.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext _context;

        public AuthorRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthors(CancellationToken cancellationToken = default)
        {
            return await _context.Authors.ToListAsync(cancellationToken);
        }

        public async Task<Author> GetAuthorById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Authors.FindAsync(id, cancellationToken);
        }

        public async Task AddAuthor(Author author, CancellationToken cancellationToken = default)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAuthor(Author author, CancellationToken cancellationToken = default)
        {
            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAuthor(int id, CancellationToken cancellationToken = default)
        {
            var author = await _context.Authors.FindAsync(new object[] { id }, cancellationToken);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken = default)
        {
            return await _context.Books.Where(b => b.AuthorId == authorId).ToListAsync(cancellationToken);
        }
    }
}
