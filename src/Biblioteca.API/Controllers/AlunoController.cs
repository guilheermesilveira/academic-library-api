using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Aluno;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class AlunoController : BaseController
{
    private readonly IAlunoService _alunoService;

    public AlunoController(INotificator notificator, IAlunoService alunoService) : base(notificator)
    {
        _alunoService = alunoService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar um novo aluno", Tags = new[] { "Administração - Alunos" })]
    [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarAlunoDto dto)
    {
        var adicionarAluno = await _alunoService.Adicionar(dto);
        return CreatedResponse("", adicionarAluno);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um aluno existente", Tags = new[] { "Administração - Alunos" })]
    [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAlunoDto dto)
    {
        var atualizarAluno = await _alunoService.Atualizar(id, dto);
        return OkResponse(atualizarAluno);
    }

    [HttpGet("pesquisar")]
    [SwaggerOperation(Summary = "Pesquisar por alunos", Tags = new[] { "Administração - Alunos" })]
    [ProducesResponseType(typeof(PaginacaoDto<AlunoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarAlunoDto dto)
    {
        var obterAlunos = await _alunoService.Pesquisar(dto);
        return OkResponse(obterAlunos);
    }

    [HttpGet("obter-todos")]
    [SwaggerOperation(Summary = "Obter todos os alunos cadastrados", Tags = new[] { "Administração - Alunos" })]
    [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterAlunos = await _alunoService.ObterTodos();
        return OkResponse(obterAlunos);
    }

    [HttpPatch("ativar/{id}")]
    [SwaggerOperation(Summary = "Ativar um aluno", Tags = new[] { "Administração - Alunos" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _alunoService.Ativar(id);
        return NoContentResponse();
    }

    [HttpPatch("desativar/{id}")]
    [SwaggerOperation(Summary = "Desativar um aluno", Tags = new[] { "Administração - Alunos" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _alunoService.Desativar(id);
        return NoContentResponse();
    }
}