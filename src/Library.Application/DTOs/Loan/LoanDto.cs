using Library.Application.DTOs.Book;
using Library.Application.DTOs.Student;

namespace Library.Application.DTOs.Loan;

public class LoanDto
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime ExpectedRepaymentDate { get; set; }
    public DateTime? RepaymentDateCompleted { get; set; }
    public string LoanStatus { get; set; } = null!;
    public int NumberOfRenewalsAllowed { get; set; }
    public int NumberOfRenewalsCompleted { get; set; }
    public bool Active { get; set; }
    public StudentDto Student { get; set; } = null!;
    public BookDto Book { get; set; } = null!;
}