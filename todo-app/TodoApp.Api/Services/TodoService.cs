using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Models;
using TodoApp.Api.Models.Dtos;

namespace TodoApp.Api.Services;

public class TodoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TodoService> _logger;

    public TodoService(AppDbContext context, ILogger<TodoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Todo>> GetAllAsync()
    {
        return await _context.Todos
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await _context.Todos.FindAsync(id);
    }

    public async Task<Todo> CreateAsync(CreateTodoDto dto)
    {
        var sanitizedTitle = SanitizeHtml(dto.Title);

        var todo = new Todo
        {
            Title = sanitizedTitle,
            Completed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created todo with ID {TodoId}", todo.Id);
        return todo;
    }

    public async Task<Todo?> UpdateAsync(int id, UpdateTodoDto dto)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return null;
        }

        if (dto.Title != null)
        {
            todo.Title = SanitizeHtml(dto.Title);
        }

        if (dto.Completed.HasValue)
        {
            todo.Completed = dto.Completed.Value;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated todo with ID {TodoId}", todo.Id);
        return todo;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return false;
        }

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted todo with ID {TodoId}", id);
        return true;
    }

    private static string SanitizeHtml(string input)
    {
        return input.Replace("<", "").Replace(">", "").Trim();
    }
}
