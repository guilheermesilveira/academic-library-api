using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class Book : Entity
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Edition { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Code { get; set; }
    public int YearOfPublication { get; set; }
    public int QuantityOfCopiesAvailableInStock { get; set; }
    public int QuantityOfCopiesAvailableForLoan { get; set; }
    public EBookStatus BookStatus { get; set; }
    public string? BookCover { get; set; }

    // Relation
    public virtual List<Loan> Loans { get; set; } = new();
}