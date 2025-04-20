using Library.Application.DTOs.Book;
using Library.Application.DTOs.Pagination;
using Microsoft.AspNetCore.Http;

namespace Library.Application.Contracts.Services;

public interface IBookService
{
    Task<BookDto?> Add(AddBookDto dto);
    Task<BookDto?> Update(int id, UpdateBookDto dto);
    Task<BookDto?> UploadCover(int id, ICollection<IFormFile>? files);
    Task<PaginationDto<BookDto>> Search(SearchBookDto dto);
    Task<List<BookDto>> GetAll();
    Task Active(int id);
    Task Inactive(int id);
}