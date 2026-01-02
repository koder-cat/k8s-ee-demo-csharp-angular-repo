using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Models.Dtos;

public class UpdateTodoDto
{
    [MinLength(1)]
    [MaxLength(500)]
    public string? Title { get; set; }

    public bool? Completed { get; set; }
}
