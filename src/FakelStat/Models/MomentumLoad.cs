using PetaPoco;

namespace FakelStat.Models;

public class MomentumLoad
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public DateOnly Day { get; set; }
    public TimeOnly Time { get; set; }
}