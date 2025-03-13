using AutoMapper;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

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

        public async Task<User> Authenticate(AuthenticateRequestModel model)
        {
            var user = await _userRepository.Authenticate(model.Username, model.Password);
            return user;
        }

        public async Task<User> Register(RegisterUserModel model)
        {
            var validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var users = await _userRepository.GetAll();
            if (await users.AnyAsync(x => x.Username == model.Username))
                throw new Exception("Username '" + model.Username + "' is already taken");

            var user = _mapper.Map<User>(model);

            var role = await _userRepository.GetRoleByName(model.Role);
            if (role == null)
                throw new Exception("Role '" + model.Role + "' does not exist");

            user.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = role
                }
            };

            var registeredUser = await _userRepository.Register(user, model.Password);
            return registeredUser;
        }

        public async Task<User> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
            return user;
        }
    }
}
