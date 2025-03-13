using AutoMapper;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace Library.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterUserModel> _validator;

        public UserService(IUserRepository userRepository, IMapper mapper, IValidator<RegisterUserModel> validator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<User> Authenticate(AuthenticateRequestModel model, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.Authenticate(model.Username, model.Password, cancellationToken);
            return user;
        }

        public async Task<User> Register(RegisterUserModel model, CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var users = await _userRepository.GetAll(cancellationToken);
            if (await users.AnyAsync(x => x.Username == model.Username, cancellationToken))
                throw new Exception("Username '" + model.Username + "' is already taken");

            var user = _mapper.Map<User>(model);

            var role = await _userRepository.GetRoleByName(model.Role, cancellationToken);
            if (role == null)
                throw new Exception("Role '" + model.Role + "' does not exist");

            user.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = role
                }
            };

            var registeredUser = await _userRepository.Register(user, model.Password, cancellationToken);
            return registeredUser;
        }

        public async Task<User> GetById(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
            return user;
        }
    }
}
