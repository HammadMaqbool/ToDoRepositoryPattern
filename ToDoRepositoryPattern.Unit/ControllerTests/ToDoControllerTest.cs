using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoRepositoryPattern.Controllers.v1;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;
using ToDoRepositoryPattern.Repositories.Interfaces;

namespace ToDoRepositoryPattern.Unit.ControllerTests;

public class ToDoControllerTest
{
    private readonly ToDoController _controller;
    private readonly Mock<IToDoRepository> _toDoRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ILogger<ToDoController>> _logger;

    private readonly Mock<IValidator<ToDoCreateDTO>> _validateCreator;
    private readonly Mock<IValidator<ToDoUpdateDTO>> _validateUpdater;

    public ToDoControllerTest()
    {
        _toDoRepository = new Mock<IToDoRepository>();
        _mapper = new Mock<IMapper>();
        _validateCreator = new Mock<IValidator<ToDoCreateDTO>>();
        _validateUpdater = new Mock<IValidator<ToDoUpdateDTO>>();
        _logger = new Mock<ILogger<ToDoController>>();

        _controller = new ToDoController(_toDoRepository.Object, 
            _mapper.Object, 
            _validateCreator.Object, 
            _validateUpdater.Object, 
            _logger.Object);
    }

    [Fact]
    public async Task GetAllToDos_ReturnsOkResult()
    {
        // Arrange
        var _mockData = new List<ToDoResponseDTO>
        {
            new ToDoResponseDTO { Title = "Test ToDo 1", Description = "Description for Test ToDo 1" },
            new ToDoResponseDTO { Title = "Test ToDo 2", Description = "Description for Test ToDo 2" }
        };
        _toDoRepository.Setup(repo => repo.GetAllToDos()).ReturnsAsync(_mockData);

        // Act
        var result = await _controller.GetAllToDos();

        // Assert
        var OkResults = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, OkResults.StatusCode);
        Assert.Equal(_mockData, OkResults.Value);

        _toDoRepository.Verify(repo => repo.GetAllToDos(), Times.Once); // Verify that the repository method was called once
    }

    [Fact]
    public async Task CreateToDo_ReturnsOkResult_WithToDo()
    {
        //Arange 
        var CreateToDoDTO = new ToDoCreateDTO { Title = "Test ToDo", Description = "Description for Test ToDo" };
        var ToDo = new ToDo { Title = CreateToDoDTO.Title, Description = CreateToDoDTO.Description };

        var ToDoResponseDTO = new ToDoResponseDTO { Title = CreateToDoDTO.Title, Description = CreateToDoDTO.Description };
        _validateCreator.Setup(v => v.Validate(CreateToDoDTO)).Returns(new FluentValidation.Results.ValidationResult());

        var MappedData = _mapper.Setup(m => m.Map<ToDo>(CreateToDoDTO)).Returns(ToDo);
        _toDoRepository.Setup(repo => repo.CreateToDo(ToDo)).ReturnsAsync(ToDoResponseDTO);

        //Act
        var Result = await _controller.CreateToDo(CreateToDoDTO);

        //Assert
        Assert.NotNull(Result); //make sure the result is not null
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        Assert.Equal(200, OkResult.StatusCode); //check if the status code is 200
        Assert.Equal(ToDo, OkResult.Value); //check if the value is the same as the expected ToDoResponseDTO
    }

    [Fact]
    public async Task CreateToDo_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var CreateToDoDTO = new ToDoCreateDTO { Title = "", Description = "Description for Test ToDo" }; // Invalid DTO with empty title
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Title", "Title is required.")
        };
        _validateCreator.Setup(v => v.Validate(CreateToDoDTO)).Returns(new FluentValidation.Results.ValidationResult(validationErrors));
        
        // Act
        var result = await _controller.CreateToDo(CreateToDoDTO);

        // Assert
        var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, BadRequestResult.StatusCode);
        Assert.Equal(validationErrors, BadRequestResult.Value); // Check if the validation errors are returned in the response
    }




}
