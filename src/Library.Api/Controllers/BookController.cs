using Library.Application.Contracts.Services;
using Library.Application.Notifications;
using Library.Api.Responses;
using Library.Application.DTOs.Book;
using Library.Application.DTOs.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Library.Api.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class BookController : BaseController
{
    private readonly IBookService _bookService;

    public BookController(INotificator notificator, IBookService bookService) : base(notificator)
    {
        _bookService = bookService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Add a new book", Tags = new[] { "Books" })]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Add([FromBody] AddBookDto dto)
    {
        var book = await _bookService.Add(dto);
        return CreatedResponse("", book);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a book", Tags = new[] { "Books" })]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
    {
        var book = await _bookService.Update(id, dto);
        return OkResponse(book);
    }

    [HttpPut("upload-cover/{id}")]
    [SwaggerOperation(Summary = "Add/update a cover for a book", Tags = new[] { "Books" })]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadCover(int id, [FromForm] ICollection<IFormFile>? files)
    {
        var book = await _bookService.UploadCover(id, files);
        return CreatedResponse("", book);
    }

    [HttpGet("search")]
    [SwaggerOperation(Summary = "Search by books", Tags = new[] { "Books" })]
    [ProducesResponseType(typeof(PaginationDto<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search([FromQuery] SearchBookDto dto)
    {
        var books = await _bookService.Search(dto);
        return OkResponse(books);
    }

    [HttpGet("get-all")]
    [SwaggerOperation(Summary = "Get all books", Tags = new[] { "Books" })]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var books = await _bookService.GetAll();
        return OkResponse(books);
    }

    [HttpPatch("active/{id}")]
    [SwaggerOperation(Summary = "Activate a book", Tags = new[] { "Books" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Active(int id)
    {
        await _bookService.Active(id);
        return NoContentResponse();
    }

    [HttpPatch("inactive/{id}")]
    [SwaggerOperation(Summary = "Deactivate a book", Tags = new[] { "Books" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inactive(int id)
    {
        await _bookService.Inactive(id);
        return NoContentResponse();
    }
}