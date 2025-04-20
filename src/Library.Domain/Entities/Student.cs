namespace Library.Domain.Entities;

public class Student : User
{
    public string Registration { get; set; } = null!;
    public string Course { get; set; } = null!;
    public int NumberOfLoansAllowed { get; set; }
    public int NumberOfLoansTaken { get; set; }
    public bool Blocked { get; set; }

    // Relation
    public virtual List<Loan> Loans { get; set; } = new();
}