using Biblioteca.Application.DTOs.Aluno;
using Biblioteca.Application.DTOs.Paginacao;

namespace Biblioteca.Application.Contracts.Services;

public interface IAlunoService
{
    Task<AlunoDto?> Adicionar(AdicionarAlunoDto dto);
    Task<AlunoDto?> Atualizar(int id, AtualizarAlunoDto dto);
    Task<PaginacaoDto<AlunoDto>> Pesquisar(PesquisarAlunoDto dto);
    Task<List<AlunoDto>> ObterTodos();
    Task Ativar(int id);
    Task Desativar(int id);
}