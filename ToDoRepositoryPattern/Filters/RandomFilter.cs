using Microsoft.AspNetCore.Mvc.Filters;

namespace ToDoRepositoryPattern.Filters;

public class RandomFilter : IActionFilter
{

    private readonly ILogger<RandomFilter> _logger;
    public RandomFilter(ILogger<RandomFilter> logger)
    {
        _logger = logger;
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("On Action Executed! {@DateTime.UtcNow}", DateTime.UtcNow);
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("On Action Executing. . . .! {DateTime.UtcNow}", DateTime.UtcNow);
    }
}
