using Library.Domain.Contracts;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Infra.Data.Context;
using Library.Infra.Data.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Library.Infra.Data.Repositories;

public class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Add(Loan loan)
        => Context.Loans.Add(loan);

    public void Update(Loan loan)
        => Context.Loans.Update(loan);

    public async Task<Loan?> GetById(int id)
    {
        return await Context.Loans
            .AsNoTracking()
            .Include(l => l.Student)
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IPagination<Loan>> Search(int? id, int? studentId, string? studentRegistration, int? bookId,
        int? bookCode, bool? active, int numberOfItemsPerPage = 10, int currentPage = 1)
    {
        var query = Context.Loans
            .AsNoTracking()
            .Include(l => l.Student)
            .Include(l => l.Book)
            .AsQueryable();

        if (id.HasValue)
            query = query.Where(l => l.Id == id);

        if (studentId.HasValue)
            query = query.Where(l => l.StudentId == studentId);

        if (!string.IsNullOrEmpty(studentRegistration))
            query = query.Where(l => l.Student.Registration.Contains(studentRegistration));

        if (bookId.HasValue)
            query = query.Where(l => l.BookId == bookId);

        if (bookCode.HasValue)
            query = query.Where(l => l.Book.Code == bookCode);

        if (active.HasValue)
            query = query.Where(l => l.Active == active);

        query = query.OrderByDescending(l => l.LoanDate);

        var result = new Pagination<Loan>
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

    public async Task<List<Loan>> GetAll()
    {
        return await Context.Loans
            .AsNoTracking()
            .Include(l => l.Student)
            .Include(l => l.Book)
            .ToListAsync();
    }
}