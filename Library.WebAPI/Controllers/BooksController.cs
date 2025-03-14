using AutoMapper;
using Library.BusinessLogic.Services;
using Library.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Library.WebAPI.Middleware;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetAllBooks(CancellationToken cancellationToken = default)
        {
            var books = await _bookService.GetAllBooks(cancellationToken);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookModel>> GetBookById(int id, CancellationToken cancellationToken = default)
        {
            var book = await _bookService.GetBookById(id, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException($"Book with ID {id} not found");
            }
            return Ok(book);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookModel>> GetBookByISBN(string isbn, CancellationToken cancellationToken = default)
        {
            var book = await _bookService.GetBookByISBN(isbn, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException($"Book with ISBN {isbn} not found");
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> AddBook(BookModel bookModel, CancellationToken cancellationToken = default)
        {
            try
            {
                await _bookService.AddBook(bookModel, cancellationToken);
                return CreatedAtAction(nameof(GetBookById), new { id = bookModel.Id }, bookModel);
            }
            catch (ConflictException ex)
            {
                throw new ConflictException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookModel bookModel, CancellationToken cancellationToken = default)
        {
            if (id != bookModel.Id)
            {
                throw new BadRequestException("Book ID mismatch");
            }
            try
            {
                await _bookService.UpdateBook(bookModel, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _bookService.DeleteBook(id, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }

        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id, [FromBody] DateTime dueDate, CancellationToken cancellationToken = default)
        {
            try
            {
                await _bookService.BorrowBook(id, dueDate, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (ConflictException ex)
            {
                throw new ConflictException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }

        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _bookService.ReturnBook(id, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddBookImage(int id, [FromBody] string imageUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                await _bookService.AddBookImage(id, imageUrl, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }
    }
}
