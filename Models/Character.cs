namespace GameApi.Models;

public sealed class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Class { get; set; }
    public int Level { get; set; } = 1;
    public int Hp { get; set; } = 100;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int AccountId { get; set; }
    public Account? Account { get; set; }
}
