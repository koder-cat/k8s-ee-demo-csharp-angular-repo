using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Models.Dtos;

public class CreateTodoDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
}
