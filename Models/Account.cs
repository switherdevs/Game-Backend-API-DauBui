namespace GameApi.Models;

public sealed class Account
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
