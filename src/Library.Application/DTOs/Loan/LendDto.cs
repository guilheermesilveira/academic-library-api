namespace Library.Application.DTOs.Loan;

public class LendDto
{
    public string StudentRegistration { get; set; } = null!;
    public string StudentPassword { get; set; } = null!;
    public int BookCode { get; set; }
}