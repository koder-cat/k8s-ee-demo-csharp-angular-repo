using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TodoApp.Api.Models;
using TodoApp.Api.Models.Dtos;
using TodoApp.Api.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]
public class TodosController : ControllerBase
{
    private readonly TodoService _todoService;

    public TodosController(TodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Todo>>> GetAll()
    {
        var todos = await _todoService.GetAllAsync();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetById(int id)
    {
        var todo = await _todoService.GetByIdAsync(id);
        if (todo == null)
        {
            return NotFound(new { message = $"Todo with ID {id} not found" });
        }
        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> Create([FromBody] CreateTodoDto dto)
    {
        var todo = await _todoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Todo>> Update(int id, [FromBody] UpdateTodoDto dto)
    {
        var todo = await _todoService.UpdateAsync(id, dto);
        if (todo == null)
        {
            return NotFound(new { message = $"Todo with ID {id} not found" });
        }
        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _todoService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Todo with ID {id} not found" });
        }
        return NoContent();
    }
}
