using Library.Application.Contracts.Services;
using Library.Application.Notifications;
using Library.Api.Responses;
using Library.Application.DTOs.Pagination;
using Library.Application.DTOs.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Library.Api.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class StudentController : BaseController
{
    private readonly IStudentService _studentService;

    public StudentController(INotificator notificator, IStudentService studentService) : base(notificator)
    {
        _studentService = studentService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Add a new student", Tags = new[] { "Students" })]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Add([FromBody] AddStudentDto dto)
    {
        var student = await _studentService.Add(dto);
        return CreatedResponse("", student);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a student", Tags = new[] { "Students" })]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
    {
        var student = await _studentService.Update(id, dto);
        return OkResponse(student);
    }

    [HttpGet("search")]
    [SwaggerOperation(Summary = "Search by students", Tags = new[] { "Students" })]
    [ProducesResponseType(typeof(PaginationDto<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search([FromQuery] SearchStudentDto dto)
    {
        var students = await _studentService.Search(dto);
        return OkResponse(students);
    }

    [HttpGet("get-all")]
    [SwaggerOperation(Summary = "Get all students", Tags = new[] { "Students" })]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAll();
        return OkResponse(students);
    }

    [HttpPatch("active/{id}")]
    [SwaggerOperation(Summary = "Activate a student", Tags = new[] { "Students" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Active(int id)
    {
        await _studentService.Active(id);
        return NoContentResponse();
    }

    [HttpPatch("inactive/{id}")]
    [SwaggerOperation(Summary = "Deactivate a student", Tags = new[] { "Students" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inactive(int id)
    {
        await _studentService.Inactive(id);
        return NoContentResponse();
    }
}