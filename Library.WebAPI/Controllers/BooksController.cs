using AutoMapper;
using Library.BusinessLogic.Services;
using Library.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                return NotFound();
            }
            return Ok(book);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookModel>> GetBookByISBN(string isbn, CancellationToken cancellationToken = default)
        {
            var book = await _bookService.GetBookByISBN(isbn, cancellationToken);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> AddBook(BookModel bookModel, CancellationToken cancellationToken = default)
        {
            await _bookService.AddBook(bookModel, cancellationToken);
            return CreatedAtAction(nameof(GetBookById), new { id = bookModel.Id }, bookModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookModel bookModel, CancellationToken cancellationToken = default)
        {
            if (id != bookModel.Id)
            {
                return BadRequest();
            }

            await _bookService.UpdateBook(bookModel, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken = default)
        {
            await _bookService.DeleteBook(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id, [FromBody] DateTime dueDate, CancellationToken cancellationToken = default)
        {
            await _bookService.BorrowBook(id, dueDate, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id, CancellationToken cancellationToken = default)
        {
            await _bookService.ReturnBook(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddBookImage(int id, [FromBody] string imageUrl, CancellationToken cancellationToken = default)
        {
            await _bookService.AddBookImage(id, imageUrl, cancellationToken);
            return NoContent();
        }
    }
}
