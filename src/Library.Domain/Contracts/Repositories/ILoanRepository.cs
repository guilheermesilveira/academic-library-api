using Library.Domain.Entities;

namespace Library.Domain.Contracts.Repositories;

public interface ILoanRepository : IRepository<Loan>
{
    void Add(Loan loan);
    void Update(Loan loan);
    Task<Loan?> GetById(int id);
    Task<IPagination<Loan>> Search(int? id, int? studentId, string? studentRegistration, int? bookId,
        int? bookCode, bool? active, int numberOfItemsPerPage = 10, int currentPage = 1);
    Task<List<Loan>> GetAll();
}