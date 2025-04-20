using Library.Application.DTOs.Loan;
using Library.Application.DTOs.Pagination;

namespace Library.Application.Contracts.Services;

public interface ILoanService
{
    Task<LoanDto?> Lend(LendDto dto);
    Task<LoanDto?> Renew(int id, RenewDto dto);
    Task<LoanDto?> Deliver(int id, DeliverDto dto);
    Task<PaginationDto<LoanDto>> Search(SearchLoanDto dto);
    Task<List<LoanDto>> GetAll();
    Task Active(int id);
    Task Inactive(int id);
}