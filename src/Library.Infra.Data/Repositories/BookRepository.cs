using Library.Domain.Contracts;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Infra.Data.Context;
using Library.Infra.Data.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Library.Infra.Data.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Add(Book book)
        => Context.Books.Add(book);

    public void Update(Book book)
        => Context.Books.Update(book);

    public async Task<IPagination<Book>> Search(int? id, string? title, string? author, string? publisher,
        string? category, int? code, bool? active, int numberOfItemsPerPage = 10, int currentPage = 1)
    {
        var query = Context.Books
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            query = query.Where(b => b.Id == id);

        if (!string.IsNullOrEmpty(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrEmpty(author))
            query = query.Where(b => b.Author.Contains(author));

        if (!string.IsNullOrEmpty(publisher))
            query = query.Where(b => b.Publisher.Contains(publisher));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(b => b.Category.Contains(category));

        if (code.HasValue)
            query = query.Where(b => b.Code == code);

        if (active.HasValue)
            query = query.Where(b => b.Active == active);

        var result = new Pagination<Book>
        {
            TotalItems = await query.CountAsync(),
            NumberOfItemsPerPage = numberOfItemsPerPage,
            CurrentPage = currentPage,
            Items = await query.Skip((currentPage - 1) * numberOfItemsPerPage).Take(numberOfItemsPerPage).ToListAsync()
        };

        var numberOfPages = (double)result.TotalItems / numberOfItemsPerPage;
        result.NumberOfPages = (int)Math.Ceiling(numberOfPages);

        return result;
    }

    public async Task<List<Book>> GetAll()
        => await Context.Books.AsNoTracking().ToListAsync();

    public IQueryable<Book> Queryable()
        => Context.Books.AsNoTracking();
}