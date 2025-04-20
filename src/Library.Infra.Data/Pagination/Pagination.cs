using Library.Domain.Contracts;
using Library.Domain.Entities;

namespace Library.Infra.Data.Pagination;

public class Pagination<T> : IPagination<T> where T : Entity, new()
{
    public int TotalItems { get; set; }
    public int NumberOfItemsPerPage { get; set; }
    public int NumberOfPages { get; set; }
    public int CurrentPage { get; set; }
    public List<T> Items { get; set; } = new();
}