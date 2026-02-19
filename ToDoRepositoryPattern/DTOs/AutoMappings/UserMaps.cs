using AutoMapper;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.DTOs.AutoMappings;

public class UserMaps : Profile
{
	public UserMaps()
	{
		CreateMap<User, UserResponseDTO>();
		CreateMap<UserCreateDTO, User>();
	}
}
