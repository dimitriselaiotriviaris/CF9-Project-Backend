namespace CF9Project.Models;

public class Game : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public string Description { get; set; } = null!;

    public int? CompanyId { get; set; }

    public Company? Company { get; set; }

    public ICollection<Gamer> Gamers { get; set; } = new HashSet<Gamer>();
}
