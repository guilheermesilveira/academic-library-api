namespace Library.Application.DTOs.Book;

public class UpdateBookDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Edition { get; set; }
    public string? Publisher { get; set; }
    public string? Category { get; set; }
    public int? YearOfPublication { get; set; }
    public int? QuantityOfCopiesAvailableInStock { get; set; }
}