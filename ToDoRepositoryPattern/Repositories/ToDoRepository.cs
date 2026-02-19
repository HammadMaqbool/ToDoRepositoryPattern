using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ToDoRepositoryPattern.Caches;
using ToDoRepositoryPattern.Data;
using ToDoRepositoryPattern.DTOs;
using ToDoRepositoryPattern.Models;
using ToDoRepositoryPattern.Repositories.Interfaces;

namespace ToDoRepositoryPattern.Repositories;

public class ToDoRepository : IToDoRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    private readonly ILogger<ToDoRepository> _logger;
    private readonly IMemoryCache _memoryCache;
    public ToDoRepository(AppDbContext context, IMapper mapper, ILogger<ToDoRepository> logger, IMemoryCache cache)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _memoryCache = cache;
    }

    public async Task<IEnumerable<ToDoResponseDTO>> GetAllToDos()
    {
        var caacheKey = CacheKey.AllToDos;
        if(!_memoryCache.TryGetValue(caacheKey, out List<ToDoResponseDTO> cachedToDos))
        {
            _logger.LogInformation("Cache miss for all ToDos. Fetching from database.");
            IQueryable<ToDoResponseDTO> GetAllToDos = _context.tbl_ToDo
            .ProjectTo<ToDoResponseDTO>(_mapper.ConfigurationProvider);

            var finalResult = await GetAllToDos.ToListAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(caacheKey, finalResult, cacheEntryOptions);

            return finalResult;
        }
        _logger.LogInformation("Cache hit for all ToDos");
        return cachedToDos;
        
    }

    public async Task<ToDo?> GetToDoById(int Id)
    {
        var cacheKey = CacheKey.ToDoById(Id);
        if(!_memoryCache.TryGetValue(cacheKey, out ToDo cachedToDo))
        {
            _logger.LogInformation($"Cache miss for ToDo with Id {Id}. Fetching from database.");
            ToDo? toDo = _context.tbl_ToDo.Find(Id);
            if (toDo is null)
            {
                return null;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(cacheKey, toDo, cacheEntryOptions);
            return toDo;
        }

        _logger.LogInformation($"Cache hit for ToDo with Id {Id}");
        return cachedToDo;
    }

    public async Task<bool> DeleteToDoById(int Id)
    {
        var cacheKey = CacheKey.ToDoById(Id);
        _memoryCache.Remove(cacheKey);


        var Record = await _context.tbl_ToDo.FindAsync(Id);

        if (Record is null)
            return false;

        _context.tbl_ToDo.Remove(Record);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ToDoResponseDTO> CreateToDo(ToDo toDo)
    {
        var cacheKey = CacheKey.AllToDos;
        _memoryCache.Remove(cacheKey);

        _context.tbl_ToDo.Add(toDo);
        await _context.SaveChangesAsync();

        return _mapper.Map<ToDoResponseDTO>(toDo);
    }

    public async Task<ToDoResponseDTO> UpdateToDo(int Id, ToDoUpdateDTO toDo)
    {

        //invalidate cache for the record being updated
        var cacheKey = CacheKey.ToDoById(Id);
        var allToDoSCacheKey = CacheKey.AllToDos;
        _memoryCache.Remove(allToDoSCacheKey);
        _memoryCache.Remove(cacheKey);

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
