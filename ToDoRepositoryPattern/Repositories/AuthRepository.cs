using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using ToDoRepositoryPattern.Caches;
using ToDoRepositoryPattern.Data;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Helpers;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.Repositories;

public class AuthRepository
{
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;
	private readonly ILogger<AuthRepository> _logger;

	private readonly IMemoryCache _cache;
	private readonly JWTService _jwtService;

	private readonly IConfiguration _configuration;

    public AuthRepository(AppDbContext context, IMapper mapper, ILogger<AuthRepository> logger, IMemoryCache cache, IConfiguration configuration)
	{
		_context = context;
		_mapper = mapper;
		_logger = logger;
		_cache = cache;
		_configuration = configuration;

		_jwtService = new JWTService(_configuration);
	}

	public async Task<UserResponseDTO> RegisterUser(User user)
	{
        //Invalidating the cache for all users when a new user is registered	
        var cahcheKey = CacheKey.AllUsers;
		_cache.Remove(cahcheKey);

		user.RefreshToken = JWTService.GenerateRefreshToken();
		user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // Set refresh token expiry to 7 days

        _context.tbl_User.Add(user);	
		await _context.SaveChangesAsync();

		return _mapper.Map<UserResponseDTO>(user);
    }

	public async Task<AuthResponseDTO> LoginUser(UserLoginDTO userLogin)
	{
		string Token = string.Empty;
		string RefreshToken = string.Empty;
        var user = _context.tbl_User.FirstOrDefault(u => u.Email == userLogin.Email);
		if(user is null || BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password) == false)
		{
			_logger.LogWarning("Failed login attempt for email: {Email}", userLogin.Email);
			throw new Exception("Invalid email or password");
		}

		if(user != null)
		{
			_logger.LogInformation("User logged in successfully & Token Generated: {Email}", user.Email);
			Token = _jwtService.GenerateToken(user);
			RefreshToken = JWTService.GenerateRefreshToken();
			user.RefreshToken = RefreshToken;
			user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // Set refresh token expiry to 7 days

			await _context.SaveChangesAsync();
        }
		return new AuthResponseDTO
		{
			JWTToken = Token,
			RefreshToken = RefreshToken
        };
	}

	public async Task<AuthResponseDTO> RefreshTokenRequest(string oldRefreshToken)
	{
		var user = _context.tbl_User.FirstOrDefault(u => u.RefreshToken == oldRefreshToken);

		if(user is null)
		{
			_logger.LogWarning("Invalid refresh token: {RefreshToken}", oldRefreshToken);
			throw new Exception("Invalid refresh token");
        }

		if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
		{
			_logger.LogWarning("Refresh token expired for user: {Email}", user.Email);
        }

		var NewAccessToken = _jwtService.GenerateToken(user);
		var NewRefreshToken = JWTService.GenerateRefreshToken();

		user.RefreshToken = NewRefreshToken;
		user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // Set new refresh token expiry to 7 days

		 //_context.tbl_User.Add(user);
		await _context.SaveChangesAsync();

		return new AuthResponseDTO
		{
			JWTToken = NewAccessToken,
			RefreshToken = NewRefreshToken
		};

    }
}
