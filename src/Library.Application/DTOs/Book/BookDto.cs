namespace Library.Application.DTOs.Book;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Edition { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Code { get; set; }
    public int YearOfPublication { get; set; }
    public int QuantityOfCopiesAvailableInStock { get; set; }
    public int QuantityOfCopiesAvailableForLoan { get; set; }
    public string BookStatus { get; set; } = null!;
    public string? BookCover { get; set; }
    public bool Active { get; set; }
}