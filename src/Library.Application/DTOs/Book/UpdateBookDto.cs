namespace Library.Application.DTOs.Book;

public class UpdateBookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Edition { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int YearOfPublication { get; set; }
    public int QuantityOfCopiesAvailableInStock { get; set; }
}