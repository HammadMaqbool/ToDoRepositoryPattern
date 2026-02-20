namespace ToDoRepositoryPattern.DTOs;

public class AuthResponseDTO
{
    public string JWTToken { get; set; }
    public string RefreshToken { get; set; }
}
