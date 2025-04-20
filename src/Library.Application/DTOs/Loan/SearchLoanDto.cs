namespace Library.Application.DTOs.Loan;

public class SearchLoanDto
{
    public int? Id { get; set; }
    public int? StudentId { get; set; }
    public string? StudentRegistration { get; set; }
    public int? BookId { get; set; }
    public int? BookCode { get; set; }
    public bool? Active { get; set; }
    public int NumberOfItemsPerPage { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;
}