using Library.DataAccess.Context;
using Library.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        public BookRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooks(CancellationToken cancellationToken = default)
        {
            return await _context.Books.ToListAsync(cancellationToken);
        }

        public async Task<Book> GetBookById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Books.FindAsync(id, cancellationToken);
        }

        public async Task<Book> GetBookByISBN(string isbn, CancellationToken cancellationToken = default)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
        }

        public async Task AddBook(Book book, CancellationToken cancellationToken = default)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateBook(Book book, CancellationToken cancellationToken = default)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBook(int id, CancellationToken cancellationToken = default)
        {
            var book = await _context.Books.FindAsync(new object[] { id }, cancellationToken);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
