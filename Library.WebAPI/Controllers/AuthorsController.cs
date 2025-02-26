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
        public async Task<ActionResult<IEnumerable<AuthorModel>>> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthors();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorModel>> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorModel>> AddAuthor(AuthorModel authorModel)
        {
            await _authorService.AddAuthor(authorModel);
            return CreatedAtAction(nameof(GetAuthorById), new { id = authorModel.Id }, authorModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, AuthorModel authorModel)
        {
            if (id != authorModel.Id)
            {
                return BadRequest();
            }

            await _authorService.UpdateAuthor(authorModel);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthor(id);
            return NoContent();
        }

        [HttpGet("{id}/books")]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooksByAuthor(int id)
        {
            var books = await _authorService.GetBooksByAuthor(id);
            return Ok(books);
        }
    }
}
