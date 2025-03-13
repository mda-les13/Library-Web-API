using AutoMapper;
using Library.BusinessLogic.Services;
using Library.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                return BadRequest(new { message = "Username or password is incorrect" });

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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token" });

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var isValid = await _tokenService.ValidateRefreshToken(refreshToken, userId, cancellationToken);
            if (!isValid)
                return Unauthorized(new { message = "Invalid token" });

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
                return Unauthorized(new { message = "Invalid token" });

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var isValid = await _tokenService.ValidateRefreshToken(refreshToken, userId, cancellationToken);
            if (!isValid)
                return Unauthorized(new { message = "Invalid token" });

            await _tokenService.RevokeRefreshToken(refreshToken, cancellationToken);

            return Ok(new { message = "Token revoked" });
        }
    }
}
