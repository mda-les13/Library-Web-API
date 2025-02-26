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

        public async Task<IEnumerable<AuthorModel>> GetAllAuthors()
        {
            var authors = await _authorRepository.GetAllAuthors();
            return _mapper.Map<IEnumerable<AuthorModel>>(authors);
        }

        public async Task<AuthorModel> GetAuthorById(int id)
        {
            var author = await _authorRepository.GetAuthorById(id);
            return _mapper.Map<AuthorModel>(author);
        }

        public async Task AddAuthor(AuthorModel authorModel)
        {
            var validationResult = _validator.Validate(authorModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var author = _mapper.Map<Author>(authorModel);
            await _authorRepository.AddAuthor(author);
        }

        public async Task UpdateAuthor(AuthorModel authorModel)
        {
            var validationResult = _validator.Validate(authorModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var author = _mapper.Map<Author>(authorModel);
            await _authorRepository.UpdateAuthor(author);
        }

        public async Task DeleteAuthor(int id)
        {
            await _authorRepository.DeleteAuthor(id);
        }

        public async Task<IEnumerable<BookModel>> GetBooksByAuthor(int authorId)
        {
            var books = await _authorRepository.GetBooksByAuthor(authorId);
            return _mapper.Map<IEnumerable<BookModel>>(books);
        }
    }
}
