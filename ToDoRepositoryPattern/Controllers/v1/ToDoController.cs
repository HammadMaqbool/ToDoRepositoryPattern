using Asp.Versioning;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;
using ToDoRepositoryPattern.Repositories.Interfaces;

namespace ToDoRepositoryPattern.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
//[Authorize(Roles ="User")]
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
    public async Task<IActionResult> GetAllToDos()
    {
        _logger.LogInformation("{DateTime.UtcNow} Getting all the ToDos", DateTimeOffset.UtcNow);
        return Ok(await _toDoRepository.GetAllToDos());

        //throw new Exception("This is a test exception to check the Serilog logging and the custom exception middleware");
    }

    [HttpPost]
    public async Task<IActionResult> CreateToDo([FromBody] ToDoCreateDTO toDoCreateDTO)
    {
        var validationStatus = _validateCreator.Validate(toDoCreateDTO);
        if (validationStatus.IsValid == false)
        {
            _logger.LogError("ToDo creation failed validation. Errors: {ValidationErrors}", validationStatus.Errors);

            return BadRequest(validationStatus.Errors);
        }
        
        ToDo toDo = _mapper.Map<ToDo>(toDoCreateDTO);

        await _toDoRepository.CreateToDo(toDo);
        return Ok(toDo);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateToDo(int Id, [FromBody] ToDoUpdateDTO toDoUpdateRequest)
    {
        var validationStatus = _validateUpdater.Validate(toDoUpdateRequest);
        if (validationStatus != null) {
            return BadRequest(validationStatus.Errors);
        }

        var finalResult = await _toDoRepository.UpdateToDo(Id, toDoUpdateRequest);

        return Ok(finalResult);
    }
}
