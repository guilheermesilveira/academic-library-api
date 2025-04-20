namespace Library.Application.DTOs.Book;

public class SearchBookDto
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Category { get; set; }
    public int? Code { get; set; }
    public bool? Active { get; set; }
    public int NumberOfItemsPerPage { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;
}