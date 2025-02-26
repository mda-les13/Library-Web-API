using AutoMapper;
using Library.DataAccess.Context;
using Library.DataAccess.Entities;
using Library.BusinessLogic.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Library.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public UserService(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> Authenticate(AuthenticateRequestModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == model.Username);

            if (user == null) return null;

            if (!VerifyPasswordHash(model.Password, user.PasswordHash)) return null;

            return user;
        }

        public async Task<User> Register(RegisterUserModel model)
        {
            if (await _context.Users.AnyAsync(x => x.Username == model.Username))
                throw new Exception("Username '" + model.Username + "' is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

            var user = _mapper.Map<User>(model);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
            if (role == null)
                throw new Exception("Role '" + model.Role + "' does not exist");

            user.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = role
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            using (var hmac = new HMACSHA512(storedHash))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i + 32]) return false;
                }
            }
            return true;
        }
    }
}
