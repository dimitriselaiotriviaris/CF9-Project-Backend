namespace CF9Project.Models;

public class Company : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    public User User { get; set; } = null!;
}
