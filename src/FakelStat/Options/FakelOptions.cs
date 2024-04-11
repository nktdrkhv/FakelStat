namespace FakelStat.Options;

public class FakelOptions
{
    public const string SECTION = "Fakel";
    public List<Header> Headers { get; set; } = null!;
    public string Token { get; set; } = null!;

    public record Header(string Title, string Value);
}