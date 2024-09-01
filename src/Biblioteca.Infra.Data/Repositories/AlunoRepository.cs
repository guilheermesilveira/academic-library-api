using Biblioteca.Domain.Contracts;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Biblioteca.Infra.Data.Paginacao;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public class AlunoRepository : Repository<Aluno>, IAlunoRepository
{
    public AlunoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Aluno aluno)
        => Context.Alunos.Add(aluno);

    public void Atualizar(Aluno aluno)
        => Context.Alunos.Update(aluno);

    public async Task<IPaginacao<Aluno>> Pesquisar(int? id, string? nome, string? email, string? matricula,
        string? curso, bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1)
    {
        var consulta = Context.Alunos
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            consulta = consulta.Where(a => a.Id == id);

        if (!string.IsNullOrEmpty(nome))
            consulta = consulta.Where(a => a.Nome.Contains(nome));

        if (!string.IsNullOrEmpty(email))
            consulta = consulta.Where(a => a.Email.Contains(email));

        if (!string.IsNullOrEmpty(matricula))
            consulta = consulta.Where(a => a.Matricula.Contains(matricula));

        if (!string.IsNullOrEmpty(curso))
            consulta = consulta.Where(a => a.Curso.Contains(curso));

        if (ativo.HasValue)
            consulta = consulta.Where(a => a.Ativo == ativo);

        var resultadoPaginado = new Paginacao<Aluno>
        {
            TotalDeItens = await consulta.CountAsync(),
            QuantidadeDeItensPorPagina = quantidadeDeItensPorPagina,
            PaginaAtual = paginaAtual,
            Itens = await consulta.Skip((paginaAtual - 1) * quantidadeDeItensPorPagina).Take(quantidadeDeItensPorPagina)
                .ToListAsync()
        };

        var quantidadeDePaginas = (double)resultadoPaginado.TotalDeItens / quantidadeDeItensPorPagina;
        resultadoPaginado.QuantidadeDePaginas = (int)Math.Ceiling(quantidadeDePaginas);

        return resultadoPaginado;
    }

    public async Task<List<Aluno>> ObterTodos()
        => await Context.Alunos.AsNoTracking().ToListAsync();
}