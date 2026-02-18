using AutoMapper;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.DTOs.AutoMappings;

public class ToDoMaps : Profile
{
	public ToDoMaps() 
	{
		CreateMap<ToDo, ToDoResponseDTO>();
		CreateMap<ToDoUpdateDTO, ToDo>();
		CreateMap<ToDoCreateDTO, ToDo>();
	}
}
