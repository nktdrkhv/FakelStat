namespace FakelStat.Models;

public class Workout
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}