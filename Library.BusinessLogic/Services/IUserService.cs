using Library.DataAccess.Entities;
using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(AuthenticateRequestModel model);
        Task<User> Register(RegisterUserModel model);
        Task<User> GetById(int id);
    }
}
