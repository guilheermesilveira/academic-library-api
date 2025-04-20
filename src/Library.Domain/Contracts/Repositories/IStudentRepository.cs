using Library.Domain.Entities;

namespace Library.Domain.Contracts.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    void Add(Student student);
    void Update(Student student);
    Task<IPagination<Student>> Search(int? id, string? name, string? email, string? registration, string? course, 
        bool? active, int numberOfItemsPerPage = 10, int currentPage = 1);
    Task<List<Student>> GetAll();
}