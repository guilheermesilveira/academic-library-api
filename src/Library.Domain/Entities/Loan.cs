using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class Loan : Entity
{
    public DateTime LoanDate { get; set; }
    public DateTime ExpectedRepaymentDate { get; set; }
    public DateTime? RepaymentDateCompleted { get; set; }
    public ELoanStatus LoanStatus { get; set; }
    public int NumberOfRenewalsAllowed { get; set; }
    public int NumberOfRenewalsCompleted { get; set; }
    public int StudentId { get; set; }
    public int BookId { get; set; }

    // Relation
    public Student Student { get; set; } = null!;
    public Book Book { get; set; } = null!;
}