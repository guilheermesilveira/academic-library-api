namespace Library.Application.Configurations;

public class JwtSettings
{
    public int ExpirationHours { get; set; }
    public string PathOfKeys { get; set; } = null!;
}