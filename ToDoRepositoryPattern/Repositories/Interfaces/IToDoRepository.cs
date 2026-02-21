using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.Repositories.Interfaces;

public interface IToDoRepository
{
    Task<ToDoResponseDTO> CreateToDo(ToDo toDo);
    Task<bool> DeleteToDoById(int Id);
    Task<IEnumerable<ToDoResponseDTO>> GetAllToDos();
    Task<ToDo?> GetToDoById(int Id);
    Task<ToDoResponseDTO> UpdateToDo(int Id, ToDoUpdateDTO toDo);
    Task<PageResponseDTO<IEnumerable<ToDo>>> GetAllToDos(PaginationDTO pagination); // V2

}
