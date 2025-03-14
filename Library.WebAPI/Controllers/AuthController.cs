using AutoMapper;
using Library.BusinessLogic.Services;
using Library.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Library.WebAPI.Middleware;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, ITokenService tokenService, IMapper mapper)
        {
            _userService = userService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequestModel model, CancellationToken cancellationToken = default)
        {
            var user = await _userService.Authenticate(model, cancellationToken);
            if (user == null)
                throw new BadRequestException("Username or password is incorrect");

            var response = _tokenService.Authenticate(user);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userService.Register(model, cancellationToken);
                return Ok(_mapper.Map<UserModel>(user));
            }
            catch (FluentValidation.ValidationException ex)
            {
                throw new ValidationFailedException(ex.Errors.Select(e => e.ErrorMessage));
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid token");

            var isValid = await _tokenService.ValidateRefreshToken(refreshToken, userId, cancellationToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid token");

            var user = await _userService.GetById(userId, cancellationToken);
            var response = _tokenService.Authenticate(user);
            await _tokenService.RevokeRefreshToken(refreshToken, cancellationToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid token");

            var isValid = await _tokenService.ValidateRefreshToken(refreshToken, userId, cancellationToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid token");

            await _tokenService.RevokeRefreshToken(refreshToken, cancellationToken);
            return Ok(new { message = "Token revoked" });
        }
    }
}
