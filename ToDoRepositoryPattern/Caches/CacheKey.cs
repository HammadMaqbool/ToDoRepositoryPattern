namespace ToDoRepositoryPattern.Caches;

public static class CacheKey
{
    public const string AllToDos = "AllToDos";
    
    public static string ToDoById(int Id) => $"ToDoById_{Id}"; //method
}
