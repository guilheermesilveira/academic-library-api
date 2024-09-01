using Biblioteca.Domain.Contracts;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Biblioteca.Infra.Data.Paginacao;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public class EmprestimoRepository : Repository<Emprestimo>, IEmprestimoRepository
{
    public EmprestimoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Emprestimo emprestimo)
        => Context.Emprestimos.Add(emprestimo);

    public void Atualizar(Emprestimo emprestimo)
        => Context.Emprestimos.Update(emprestimo);

    public async Task<Emprestimo?> ObterPorId(int id)
    {
        return await Context.Emprestimos
            .AsNoTracking()
            .Include(e => e.Aluno)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IPaginacao<Emprestimo>> Pesquisar(int? id, int? alunoId, string? alunoMatricula, int? livroId,
        int? livroCodigo, bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1)
    {
        var consulta = Context.Emprestimos
            .AsNoTracking()
            .Include(e => e.Aluno)
            .Include(e => e.Livro)
            .AsQueryable();

        if (id.HasValue)
            consulta = consulta.Where(e => e.Id == id);

        if (alunoId.HasValue)
            consulta = consulta.Where(e => e.AlunoId == alunoId);

        if (!string.IsNullOrEmpty(alunoMatricula))
            consulta = consulta.Where(e => e.Aluno.Matricula.Contains(alunoMatricula));

        if (livroId.HasValue)
            consulta = consulta.Where(e => e.LivroId == livroId);

        if (livroCodigo.HasValue)
            consulta = consulta.Where(e => e.Livro.Codigo == livroCodigo);

        if (ativo.HasValue)
            consulta = consulta.Where(e => e.Ativo == ativo);

        consulta = consulta.OrderByDescending(e => e.DataEmprestimo);

        var resultadoPaginado = new Paginacao<Emprestimo>
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

    public async Task<List<Emprestimo>> ObterTodos()
    {
        return await Context.Emprestimos
            .AsNoTracking()
            .Include(e => e.Aluno)
            .Include(e => e.Livro)
            .ToListAsync();
    }
}