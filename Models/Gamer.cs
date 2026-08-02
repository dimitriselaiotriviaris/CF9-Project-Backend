namespace CF9Project.Models;

public class Gamer : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Game> Games { get; set; } = new HashSet<Game>();
}
