namespace ToDoRepositoryPattern.Caches;

public static class CacheKey
{
    public const string AllToDos = "AllToDos";
    
    public static string ToDoById(int Id) => $"ToDoById_{Id}"; //method

    public const string AllUsers = "AllUsers"; 
    public static string UserById(int Id) => $"UserById_{Id}"; //method
}
