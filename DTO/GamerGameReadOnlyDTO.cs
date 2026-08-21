namespace CF9Project.DTO;

public record GamerGameReadOnlyDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public string? Description { get; set; }
    public string CompanyUsername { get; set; } = string.Empty;
}