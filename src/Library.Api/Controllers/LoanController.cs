using Library.Application.Contracts.Services;
using Library.Application.Notifications;
using Library.Api.Responses;
using Library.Application.DTOs.Loan;
using Library.Application.DTOs.Pagination;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Library.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
public class LoanController : BaseController
{
    private readonly ILoanService _loanService;

    public LoanController(INotificator notificator, ILoanService loanService) : base(notificator)
    {
        _loanService = loanService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Add a new loan", Tags = new[] { "Loans" })]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Lend([FromBody] LendDto dto)
    {
        var loan = await _loanService.Lend(dto);
        return CreatedResponse("", loan);
    }

    [HttpPut("renew/{id}")]
    [SwaggerOperation(Summary = "Renew a loan", Tags = new[] { "Loans" })]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Renew(int id, [FromBody] RenewDto dto)
    {
        var loan = await _loanService.Renew(id, dto);
        return OkResponse(loan);
    }

    [HttpPut("deliver/{id}")]
    [SwaggerOperation(Summary = "Deliver a loan", Tags = new[] { "Loans" })]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deliver(int id, [FromBody] DeliverDto dto)
    {
        var loan = await _loanService.Deliver(id, dto);
        return OkResponse(loan);
    }

    [HttpGet("search")]
    [SwaggerOperation(Summary = "Search by loans", Tags = new[] { "Loans" })]
    [ProducesResponseType(typeof(PaginationDto<LoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search([FromQuery] SearchLoanDto dto)
    {
        var loans = await _loanService.Search(dto);
        return OkResponse(loans);
    }

    [HttpGet("get-all")]
    [SwaggerOperation(Summary = "Get all loans", Tags = new[] { "Loans" })]
    [ProducesResponseType(typeof(List<LoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var loans = await _loanService.GetAll();
        return OkResponse(loans);
    }

    [HttpPatch("active/{id}")]
    [SwaggerOperation(Summary = "Activate a loan", Tags = new[] { "Loans" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Active(int id)
    {
        await _loanService.Active(id);
        return NoContentResponse();
    }

    [HttpPatch("inactive/{id}")]
    [SwaggerOperation(Summary = "Deactivate a loan", Tags = new[] { "Loans" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inactive(int id)
    {
        await _loanService.Inactive(id);
        return NoContentResponse();
    }
}