using Library.DataAccess.Entities;

namespace Library.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password, CancellationToken cancellationToken = default);
        Task<User> Register(User user, string password, CancellationToken cancellationToken = default);
        Task<User> GetById(int id, CancellationToken cancellationToken = default);
        Task<IQueryable<User>> GetAll(CancellationToken cancellationToken = default);
        Task<Role> GetRoleByName(string roleName, CancellationToken cancellationToken = default);
    }
}
