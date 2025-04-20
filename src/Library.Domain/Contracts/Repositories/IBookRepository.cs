using Library.Domain.Entities;

namespace Library.Domain.Contracts.Repositories;

public interface IBookRepository : IRepository<Book>
{
    void Add(Book book);
    void Update(Book book);
    Task<IPagination<Book>> Search(int? id, string? title, string? author, string? publisher, string? category,
        int? code, bool? active, int numberOfItemsPerPage = 10, int currentPage = 1);
    Task<List<Book>> GetAll();
    IQueryable<Book> Queryable();
}