using Library.Domain.Contracts;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Infra.Data.Context;
using Library.Infra.Data.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Library.Infra.Data.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Add(Student student)
        => Context.Students.Add(student);

    public void Update(Student student)
        => Context.Students.Update(student);

    public async Task<IPagination<Student>> Search(int? id, string? name, string? email, string? registration,
        string? course, bool? active, int numberOfItemsPerPage = 10, int currentPage = 1)
    {
        var query = Context.Students
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            query = query.Where(s => s.Id == id);

        if (!string.IsNullOrEmpty(name))
            query = query.Where(s => s.Name.Contains(name));

        if (!string.IsNullOrEmpty(email))
            query = query.Where(s => s.Email.Contains(email));

        if (!string.IsNullOrEmpty(registration))
            query = query.Where(s => s.Registration.Contains(registration));

        if (!string.IsNullOrEmpty(course))
            query = query.Where(s => s.Course.Contains(course));

        if (active.HasValue)
            query = query.Where(s => s.Active == active);

        var result = new Pagination<Student>
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

    public async Task<List<Student>> GetAll()
        => await Context.Students.AsNoTracking().ToListAsync();
}