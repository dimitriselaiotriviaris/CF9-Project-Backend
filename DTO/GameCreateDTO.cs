using System.ComponentModel.DataAnnotations;

namespace CF9Project.DTO;

public record GameCreateDTO
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    public int Price { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}