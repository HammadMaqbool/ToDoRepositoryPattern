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

		_context.tbl_User.Add(user);	
		await _context.SaveChangesAsync();

		return _mapper.Map<UserResponseDTO>(user);
    }

	public async Task<string> LoginUser(UserLoginDTO userLogin)
	{
		string Token = string.Empty;
        var user = _context.tbl_User.FirstOrDefault(u => u.Email == userLogin.Email && u.Password == userLogin.Password);
		if(user != null)
		{
			_logger.LogInformation("User logged in successfully & Token Generated: {Email}", user.Email);
			Token = _jwtService.GenerateToken(user);
        }
		return Token;
	}

    
}
