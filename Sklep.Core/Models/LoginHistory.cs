namespace Sklep.Core.Models;

public class LoginHistory
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public DateTime LoginTime { get; set; } = DateTime.Now;

    public bool Success { get; set; }
}