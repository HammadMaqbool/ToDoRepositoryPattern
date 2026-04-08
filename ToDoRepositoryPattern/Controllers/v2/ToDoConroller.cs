using Asp.Versioning;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;
using ToDoRepositoryPattern.Repositories.Interfaces;

namespace ToDoRepositoryPattern.Controllers.v2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ToDoController : Controller
{
    private readonly IToDoRepository _toDoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ToDoController> _logger;

    private readonly IValidator<ToDoCreateDTO> _validateCreator;
    private readonly IValidator<ToDoUpdateDTO> _validateUpdater;
    public ToDoController(IToDoRepository toDoRepository, IMapper mapper, IValidator<ToDoCreateDTO> validateCreator, IValidator<ToDoUpdateDTO> validateUpdater, ILogger<ToDoController> logger)
    {
        _toDoRepository = toDoRepository;
        _mapper = mapper;
        _validateCreator = validateCreator;
        _validateUpdater = validateUpdater;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllToDos([FromQuery] PaginationDTO paginationDTO)
    {
        _logger.LogInformation("{DateTime.UtcNow} Getting all the ToDos", DateTimeOffset.UtcNow);

        var toDos = await _toDoRepository.GetAllToDos(paginationDTO);
        return Ok(toDos);

        //throw new Exception("This is a test exception to check the Serilog logging and the custom exception middleware");
    }

}
