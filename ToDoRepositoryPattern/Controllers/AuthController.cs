using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;
using ToDoRepositoryPattern.Repositories;

namespace ToDoRepositoryPattern.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthRepository _authRepository;
    private readonly IMapper _mapper;
    public AuthController(AuthRepository authRepository, IMapper mapper)
    {
        _authRepository = authRepository;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDTO userCreateDTO)
    {
        var user = _mapper.Map<User>(userCreateDTO);
        return Ok(await _authRepository.RegisterUser(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
    {
        return Ok(await _authRepository.LoginUser(userLoginDTO));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string RefreshTokenRequest)
    {
        var response = await _authRepository.RefreshTokenRequest(RefreshTokenRequest);
        if (response == null)
        {
            return BadRequest("Invalid refresh token");
        }
        return Ok(response);
    }
}