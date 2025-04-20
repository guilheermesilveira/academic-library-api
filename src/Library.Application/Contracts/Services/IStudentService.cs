using Library.Application.DTOs.Pagination;
using Library.Application.DTOs.Student;

namespace Library.Application.Contracts.Services;

public interface IStudentService
{
    Task<StudentDto?> Add(AddStudentDto dto);
    Task<StudentDto?> Update(int id, UpdateStudentDto dto);
    Task<PaginationDto<StudentDto>> Search(SearchStudentDto dto);
    Task<List<StudentDto>> GetAll();
    Task Active(int id);
    Task Inactive(int id);
}