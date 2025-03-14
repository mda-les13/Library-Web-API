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
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorService authorService, IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorModel>>> GetAllAuthors(CancellationToken cancellationToken = default)
        {
            var authors = await _authorService.GetAllAuthors(cancellationToken);
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorModel>> GetAuthorById(int id, CancellationToken cancellationToken = default)
        {
            var author = await _authorService.GetAuthorById(id, cancellationToken);
            if (author == null)
            {
                throw new NotFoundException($"Author with ID {id} not found");
            }
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorModel>> AddAuthor(AuthorModel authorModel, CancellationToken cancellationToken = default)
        {
            try
            {
                await _authorService.AddAuthor(authorModel, cancellationToken);
                return CreatedAtAction(nameof(GetAuthorById), new { id = authorModel.Id }, authorModel);
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
        public async Task<IActionResult> UpdateAuthor(int id, AuthorModel authorModel, CancellationToken cancellationToken = default)
        {
            if (id != authorModel.Id)
            {
                throw new BadRequestException("Author ID mismatch");
            }
            try
            {
                await _authorService.UpdateAuthor(authorModel, cancellationToken);
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
        public async Task<IActionResult> DeleteAuthor(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _authorService.DeleteAuthor(id, cancellationToken);
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

        [HttpGet("{id}/books")]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooksByAuthor(int id, CancellationToken cancellationToken = default)
        {
            var books = await _authorService.GetBooksByAuthor(id, cancellationToken);
            return Ok(books);
        }
    }
}
