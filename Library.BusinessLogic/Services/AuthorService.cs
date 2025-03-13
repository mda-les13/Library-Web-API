using AutoMapper;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.BusinessLogic.Models;
using FluentValidation;

namespace Library.BusinessLogic.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<AuthorModel> _validator;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper, IValidator<AuthorModel> validator)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<AuthorModel>> GetAllAuthors(CancellationToken cancellationToken = default)
        {
            var authors = await _authorRepository.GetAllAuthors(cancellationToken);
            return _mapper.Map<IEnumerable<AuthorModel>>(authors);
        }

        public async Task<AuthorModel> GetAuthorById(int id, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetAuthorById(id, cancellationToken);
            if (author == null)
            {
                throw new KeyNotFoundException($"Author with ID {id} not found.");
            }
            return _mapper.Map<AuthorModel>(author);
        }

        public async Task AddAuthor(AuthorModel authorModel, CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(authorModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var author = _mapper.Map<Author>(authorModel);
            await _authorRepository.AddAuthor(author, cancellationToken);
        }

        public async Task UpdateAuthor(AuthorModel authorModel, CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(authorModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var author = await _authorRepository.GetAuthorById(authorModel.Id, cancellationToken);
            if (author == null)
            {
                throw new KeyNotFoundException($"Author with ID {authorModel.Id} not found.");
            }

            var updatedAuthor = _mapper.Map<Author>(authorModel);
            await _authorRepository.UpdateAuthor(updatedAuthor, cancellationToken);
        }

        public async Task DeleteAuthor(int id, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetAuthorById(id, cancellationToken);
            if (author == null)
            {
                throw new KeyNotFoundException($"Author with ID {id} not found.");
            }

            await _authorRepository.DeleteAuthor(id, cancellationToken);
        }

        public async Task<IEnumerable<BookModel>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetAuthorById(authorId, cancellationToken);
            if (author == null)
            {
                throw new KeyNotFoundException($"Author with ID {authorId} not found.");
            }

            var books = await _authorRepository.GetBooksByAuthor(authorId, cancellationToken);
            return _mapper.Map<IEnumerable<BookModel>>(books);
        }
    }
}
