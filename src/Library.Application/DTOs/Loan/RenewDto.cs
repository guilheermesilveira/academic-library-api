namespace Library.Application.DTOs.Loan;

public class RenewDto
{
    public int Id { get; set; }
    public string StudentRegistration { get; set; } = null!;
    public string StudentPassword { get; set; } = null!;
    public int BookCode { get; set; }
}