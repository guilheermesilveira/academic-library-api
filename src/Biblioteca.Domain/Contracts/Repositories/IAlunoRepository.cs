using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    void Adicionar(Aluno aluno);
    void Atualizar(Aluno aluno);
    Task<IPaginacao<Aluno>> Pesquisar(int? id, string? nome, string? email, string? matricula, string? curso,
        bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1);
    Task<List<Aluno>> ObterTodos();
}