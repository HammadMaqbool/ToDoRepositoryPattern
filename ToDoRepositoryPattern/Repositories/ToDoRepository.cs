using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoRepositoryPattern.Data;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;
using ToDoRepositoryPattern.Repositories.Interfaces;

namespace ToDoRepositoryPattern.Repositories;

public class ToDoRepository : IToDoRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public ToDoRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ToDoResponseDTO>> GetAllToDos()
    {
        IQueryable<ToDoResponseDTO> GetAllToDos = _context.tbl_ToDo
            .ProjectTo<ToDoResponseDTO>(_mapper.ConfigurationProvider); 

        return await GetAllToDos.ToListAsync();
    }

    public async Task<ToDo?> GetToDoById(int Id)
    {
        ToDo? toDo = _context.tbl_ToDo.Find(Id);
        return toDo;
    }

    public async Task<bool> DeleteToDoById(int Id)
    {
        var Record = await _context.tbl_ToDo.FindAsync(Id);

        if (Record is null)
            return false;

        _context.tbl_ToDo.Remove(Record);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ToDoResponseDTO> CreateToDo(ToDo toDo)
    {
        _context.tbl_ToDo.Add(toDo);
        await _context.SaveChangesAsync();

        return _mapper.Map<ToDoResponseDTO>(toDo);
    }

    public async Task<ToDoResponseDTO> UpdateToDo(int Id, ToDoUpdateDTO toDo)
    {
        var recordToUpdate = _context.tbl_ToDo.Find(Id);

        if (recordToUpdate is null)
            throw new Exception($"Record for the id {Id} was not found");

        _mapper.Map(toDo, recordToUpdate);

        recordToUpdate.UpdatedAt = DateTimeOffset.UtcNow;

        _context.tbl_ToDo.Update(recordToUpdate);
        await _context.SaveChangesAsync();

        return _mapper.Map<ToDoResponseDTO>(recordToUpdate);
    }
}
