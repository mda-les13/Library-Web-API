using Library.DataAccess.Entities;
using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(AuthenticateRequestModel model, CancellationToken cancellationToken = default);
        Task<User> Register(RegisterUserModel model, CancellationToken cancellationToken = default);
        Task<User> GetById(int id, CancellationToken cancellationToken = default);
    }
}
