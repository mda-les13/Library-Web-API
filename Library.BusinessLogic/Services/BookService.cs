﻿using AutoMapper;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.BusinessLogic.Models;
using FluentValidation;

namespace Library.BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<BookModel> _validator;

        public BookService(IBookRepository bookRepository, IMapper mapper, IValidator<BookModel> validator)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<BookModel>> GetAllBooks()
        {
            var books = await _bookRepository.GetAllBooks();
            return _mapper.Map<IEnumerable<BookModel>>(books);
        }

        public async Task<BookModel> GetBookById(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            return _mapper.Map<BookModel>(book);
        }

        public async Task<BookModel> GetBookByISBN(string isbn)
        {
            var book = await _bookRepository.GetBookByISBN(isbn);
            return _mapper.Map<BookModel>(book);
        }

        public async Task AddBook(BookModel bookModel)
        {
            var validationResult = _validator.Validate(bookModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var book = _mapper.Map<Book>(bookModel);
            await _bookRepository.AddBook(book);
        }

        public async Task UpdateBook(BookModel bookModel)
        {
            var validationResult = _validator.Validate(bookModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var book = _mapper.Map<Book>(bookModel);
            await _bookRepository.UpdateBook(book);
        }

        public async Task DeleteBook(int id)
        {
            await _bookRepository.DeleteBook(id);
        }

        public async Task BorrowBook(int bookId, DateTime dueDate)
        {
            var book = await _bookRepository.GetBookById(bookId);
            if (book != null)
            {
                book.BorrowedDate = DateTime.Now;
                book.DueDate = dueDate;
                await _bookRepository.UpdateBook(book);
            }
        }

        public async Task ReturnBook(int bookId)
        {
            var book = await _bookRepository.GetBookById(bookId);
            if (book != null)
            {
                book.BorrowedDate = null;
                book.DueDate = null;
                await _bookRepository.UpdateBook(book);
            }
        }

        public async Task AddBookImage(int bookId, string imageUrl)
        {
            var book = await _bookRepository.GetBookById(bookId);
            if (book != null)
            {
                book.ImageUrl = imageUrl;
                await _bookRepository.UpdateBook(book);
            }
        }
    }
}
