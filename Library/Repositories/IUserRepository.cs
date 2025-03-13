using Library.DataAccess.Entities;

namespace Library.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Register(User user, string password);
        Task<User> GetById(int id);
        Task<IQueryable<User>> GetAll();
        Task<Role> GetRoleByName(string roleName);
    }
}
