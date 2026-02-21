namespace ToDoRepositoryPattern.DTOs;

public class PaginationDTO
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 1; //should be atleast  10
}
