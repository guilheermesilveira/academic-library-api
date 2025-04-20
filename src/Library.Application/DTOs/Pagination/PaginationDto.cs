namespace Library.Application.DTOs.Pagination;

public class PaginationDto<T>
{
    public int TotalItems { get; set; }
    public int NumberOfItemsPerPage { get; set; }
    public int NumberOfPages { get; set; }
    public int CurrentPage { get; set; }
    public List<T> Items { get; set; } = new();
}