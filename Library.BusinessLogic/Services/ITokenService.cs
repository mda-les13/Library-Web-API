using Library.DataAccess.Entities;
using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Services
{
    public interface ITokenService
    {
        AuthenticateResponseModel Authenticate(User user);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        Task<bool> ValidateRefreshToken(string refreshToken, int userId, CancellationToken cancellationToken = default);
        Task RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken = default);
    }
}
