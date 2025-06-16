using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;  // Your ApplicationUser class namespace
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;          // Response<T> class namespace
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace SmartApp.WebApi.Controllers.Auth
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper=mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(Response<string>.Failure("Invalid registration data."));
            var createObj = _mapper.Map<ApplicationUser>(model);
            createObj.isActive=true;
            var result = await _userManager.CreateAsync(createObj, model.Password);

            if (!result.Succeeded)
                return BadRequest(Response<string>.Failure(string.Join("; ", result.Errors.Select(e => e.Description))));

            return Ok(Response<ApplicationUser>.SuccessResponse(createObj, "User registered successfully."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(Response<string>.Failure("Invalid login data."));

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return Unauthorized(Response<string>.Failure("Invalid username or password."));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized(Response<string>.Failure("Invalid username or password."));

            var token = _tokenService.GenerateToken(user);

            var userDto =_mapper.Map<ApplicationUserDto>(user);

            var response = new LoginResponseDto
            {
                Token = token,
                User = userDto
            };

            return Ok(Response<LoginResponseDto>.SuccessResponse(response, "Login successful."));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(Response<string>.SuccessResponse(null, "Logout successful."));
        }
    }
}



