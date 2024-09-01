using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IEmprestimoRepository : IRepository<Emprestimo>
{
    void Adicionar(Emprestimo emprestimo);
    void Atualizar(Emprestimo emprestimo);
    Task<Emprestimo?> ObterPorId(int id);
    Task<IPaginacao<Emprestimo>> Pesquisar(int? id, int? alunoId, string? alunoMatricula, int? livroId,
        int? livroCodigo, bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1);
    Task<List<Emprestimo>> ObterTodos();
}