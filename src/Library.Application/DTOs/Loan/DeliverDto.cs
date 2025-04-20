namespace Library.Application.DTOs.Loan;

public class DeliverDto
{
    public int Id { get; set; }
    public string StudentRegistration { get; set; } = null!;
    public int BookCode { get; set; }
}