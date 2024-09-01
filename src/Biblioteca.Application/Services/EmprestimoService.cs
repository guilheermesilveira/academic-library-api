using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Application.Services;

public class EmprestimoService : BaseService, IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILivroRepository _livroRepository;
    private readonly IPasswordHasher<Aluno> _passwordHasher;

    public EmprestimoService(INotificator notificator, IMapper mapper, IEmprestimoRepository emprestimoRepository,
        IAlunoRepository alunoRepository, ILivroRepository livroRepository,
        IPasswordHasher<Aluno> passwordHasher) : base(notificator, mapper)
    {
        _emprestimoRepository = emprestimoRepository;
        _alunoRepository = alunoRepository;
        _livroRepository = livroRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<EmprestimoDto?> RealizarEmprestimo(RealizarEmprestimoDto dto)
    {
        if (!await ValidacoesParaRealizarEmprestimo(dto))
            return null;

        var aluno = await _alunoRepository.FirstOrDefault(a => a.Matricula == dto.AlunoMatricula);
        var livro = await _livroRepository.FirstOrDefault(l => l.Codigo == dto.LivroCodigo);

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(aluno!, aluno!.Senha, dto.AlunoSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        aluno.QuantidadeEmprestimosRealizados += 1;
        _alunoRepository.Atualizar(aluno);

        livro!.QuantidadeExemplaresDisponiveisParaEmprestimo -= 1;
        if (livro.QuantidadeExemplaresDisponiveisParaEmprestimo == 0)
        {
            livro.StatusLivro = EStatusLivro.Indisponivel;
        }
        _livroRepository.Atualizar(livro);

        var emprestimo = new Emprestimo();
        emprestimo.DataEmprestimo = DateTime.Today;
        emprestimo.DataDevolucaoPrevista = DateTime.Today.AddDays(10);
        emprestimo.StatusEmprestimo = EStatusEmprestimo.Emprestado;
        emprestimo.QuantidadeRenovacoesPermitida = 5;
        emprestimo.QuantidadeRenovacoesRealizadas = 0;
        emprestimo.AlunoId = aluno.Id;
        emprestimo.LivroId = livro.Id;
        emprestimo.Ativo = true;
        _emprestimoRepository.Adicionar(emprestimo);

        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> RealizarRenovacao(int id, RealizarRenovacaoDto dto)
    {
        if (!await ValidacoesParaRealizarRenovacao(id, dto))
            return null;

        var emprestimo = await _emprestimoRepository.ObterPorId(id);
        var aluno = emprestimo!.Aluno;

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(aluno, aluno.Senha, dto.AlunoSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        emprestimo.DataEmprestimo = DateTime.Today;
        emprestimo.DataDevolucaoPrevista = DateTime.Today.AddDays(10);
        emprestimo.StatusEmprestimo = EStatusEmprestimo.Renovado;
        emprestimo.QuantidadeRenovacoesRealizadas += 1;
        _emprestimoRepository.Atualizar(emprestimo);
        
        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> RealizarEntrega(int id, RealizarEntregaDto dto)
    {
        if (!await ValidacoesParaRealizarEntrega(id, dto))
            return null;

        var emprestimo = await _emprestimoRepository.ObterPorId(id);
        var aluno = emprestimo!.Aluno;
        var livro = emprestimo.Livro;

        aluno.QuantidadeEmprestimosRealizados -= 1;
        _alunoRepository.Atualizar(aluno);

        if (livro.StatusLivro == EStatusLivro.Indisponivel)
        {
            livro.StatusLivro = EStatusLivro.Disponivel;
        }
        livro.QuantidadeExemplaresDisponiveisParaEmprestimo += 1;
        _livroRepository.Atualizar(livro);

        emprestimo.Ativo = false;
        emprestimo.DataDevolucaoRealizada = DateTime.Today;
        emprestimo.StatusEmprestimo = emprestimo.DataDevolucaoRealizada > emprestimo.DataDevolucaoPrevista
            ? EStatusEmprestimo.EntregueComAtraso
            : EStatusEmprestimo.Entregue;
        _emprestimoRepository.Atualizar(emprestimo);

        if (!await CommitChanges())
            return null;

        if (aluno.Bloqueado)
        {
            var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
                e.AlunoId == aluno.Id &&
                DateTime.Today > e.DataDevolucaoPrevista &&
                (e.StatusEmprestimo == EStatusEmprestimo.Emprestado ||
                 e.StatusEmprestimo == EStatusEmprestimo.Renovado));
            if (emprestimoAtrasado == null)
            {
                aluno.Bloqueado = false;
                _alunoRepository.Atualizar(aluno);
                await _alunoRepository.UnitOfWork.Commit();
            }
        }

        return Mapper.Map<EmprestimoDto>(emprestimo);
    }

    public async Task<PaginacaoDto<EmprestimoDto>> Pesquisar(PesquisarEmprestimoDto dto)
    {
        var resultadoPaginado = await _emprestimoRepository.Pesquisar(dto.Id, dto.AlunoId,
            dto.AlunoMatricula, dto.LivroId, dto.LivroCodigo, dto.Ativo, dto.QuantidadeDeItensPorPagina,
            dto.PaginaAtual);

        return new PaginacaoDto<EmprestimoDto>
        {
            TotalDeItens = resultadoPaginado.TotalDeItens,
            QuantidadeDeItensPorPagina = resultadoPaginado.QuantidadeDeItensPorPagina,
            QuantidadeDePaginas = resultadoPaginado.QuantidadeDePaginas,
            PaginaAtual = resultadoPaginado.PaginaAtual,
            Itens = Mapper.Map<List<EmprestimoDto>>(resultadoPaginado.Itens)
        };
    }

    public async Task<List<EmprestimoDto>> ObterTodos()
    {
        var emprestimos = await _emprestimoRepository.ObterTodos();
        return Mapper.Map<List<EmprestimoDto>>(emprestimos);
    }

    public async Task Ativar(int id)
    {
        var emprestimo = await _emprestimoRepository.FirstOrDefault(e => e.Id == id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (emprestimo.Ativo)
        {
            Notificator.Handle("O empréstimo informado já está ativado.");
            return;
        }

        emprestimo.Ativo = true;
        _emprestimoRepository.Atualizar(emprestimo);

        await CommitChanges();
    }

    public async Task Desativar(int id)
    {
        var emprestimo = await _emprestimoRepository.FirstOrDefault(e => e.Id == id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (emprestimo.Ativo == false)
        {
            Notificator.Handle("O empréstimo informado já está desativado.");
            return;
        }

        if (emprestimo.StatusEmprestimo != EStatusEmprestimo.Entregue &&
            emprestimo.StatusEmprestimo != EStatusEmprestimo.EntregueComAtraso)
        {
            Notificator.Handle("O empréstimo não pode ser desativado, pois o mesmo ainda não foi entregue.");
            return;
        }

        emprestimo.Ativo = false;
        _emprestimoRepository.Atualizar(emprestimo);
        
        await CommitChanges();
    }

    private async Task<bool> ValidacoesParaRealizarEmprestimo(RealizarEmprestimoDto dto)
    {
        var aluno = await _alunoRepository.FirstOrDefault(a => a.Matricula == dto.AlunoMatricula);
        if (aluno == null)
        {
            Notificator.Handle("Aluno não encontrado com a matrícula informada.");
            return false;
        }

        if (aluno.Ativo == false)
        {
            Notificator.Handle("Não é possível realizar um empréstimo para um aluno desativado.");
            return false;
        }

        var livro = await _livroRepository.FirstOrDefault(l => l.Codigo == dto.LivroCodigo);
        if (livro == null)
        {
            Notificator.Handle("Livro não encontrado com o código informado.");
            return false;
        }

        if (livro.Ativo == false)
        {
            Notificator.Handle("Não é possível realizar um empréstimo para um livro desativado.");
            return false;
        }

        if (aluno.QuantidadeEmprestimosRealizados == aluno.QuantidadeEmprestimosPermitida)
        {
            Notificator.Handle("O aluno já atingiu o limite de empréstimos.");
            return false;
        }

        if (livro.StatusLivro == EStatusLivro.Indisponivel)
        {
            Notificator.Handle("Não existe exemplar disponível no momento para esse livro.");
            return false;
        }

        if (aluno.Bloqueado)
        {
            Notificator.Handle("O aluno está temporariamente impedido de realizar empréstimos ou renovações.");
            return false;
        }

        var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
            e.AlunoId == aluno.Id &&
            DateTime.Today > e.DataDevolucaoPrevista &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoAtrasado != null)
        {
            aluno.Bloqueado = true;
            _alunoRepository.Atualizar(aluno);
            await _alunoRepository.UnitOfWork.Commit();

            Notificator.Handle("O aluno possui livro(s) não devolvido(s) e atrasado(s). " +
                               "O aluno será impedido de fazer empréstimos/renovações até devolvê-lo(s).");

            return false;
        }

        var emprestimoIgual = await _emprestimoRepository.FirstOrDefault(e =>
            e.LivroId == livro.Id &&
            e.AlunoId == aluno.Id &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoIgual != null)
        {
            Notificator.Handle("No momento o aluno já possui um exemplar emprestado ou renovado desse mesmo livro.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaRealizarRenovacao(int id, RealizarRenovacaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return false;
        }

        var emprestimo = await _emprestimoRepository.ObterPorId(id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (emprestimo.StatusEmprestimo is EStatusEmprestimo.Entregue or EStatusEmprestimo.EntregueComAtraso)
        {
            Notificator.Handle("O empréstimo já está como entregue. Não é possível renovar o livro.");
            return false;
        }

        if (emprestimo.QuantidadeRenovacoesRealizadas == emprestimo.QuantidadeRenovacoesPermitida)
        {
            Notificator.Handle("O aluno já atingiu o limite de renovações para esse livro.");
            return false;
        }

        var alunoRegistradoNoEmprestimo = await _alunoRepository.FirstOrDefault(a =>
            a.Matricula == emprestimo.Aluno.Matricula);
        var alunoInformadoNaDto = await _alunoRepository.FirstOrDefault(a =>
            a.Matricula == dto.AlunoMatricula);

        if (alunoInformadoNaDto == null)
        {
            Notificator.Handle("Aluno não encontrado com a matrícula informada.");
            return false;
        }

        if (alunoRegistradoNoEmprestimo!.Matricula != alunoInformadoNaDto.Matricula)
        {
            Notificator.Handle("A matrícula informada não pertence ao aluno que foi registrado para esse empréstimo.");
            return false;
        }

        var livroRegistradoNoEmprestimo = await _livroRepository.FirstOrDefault(l =>
            l.Codigo == emprestimo.Livro.Codigo);
        var livroInformadoNaDto = await _livroRepository.FirstOrDefault(l =>
            l.Codigo == dto.LivroCodigo);

        if (livroInformadoNaDto == null)
        {
            Notificator.Handle("Livro não encontrado com o código informado.");
            return false;
        }

        if (livroRegistradoNoEmprestimo!.Codigo != livroInformadoNaDto.Codigo)
        {
            Notificator.Handle("O código do livro informado não pertence ao livro que foi registrado para esse empréstimo.");
            return false;
        }

        if (alunoInformadoNaDto.Bloqueado)
        {
            Notificator.Handle("O aluno está temporariamente impedido de realizar empréstimos ou renovações.");
            return false;
        }

        var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
            e.AlunoId == alunoInformadoNaDto.Id &&
            DateTime.Today > e.DataDevolucaoPrevista &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoAtrasado != null)
        {
            alunoInformadoNaDto.Bloqueado = true;
            _alunoRepository.Atualizar(alunoInformadoNaDto);
            await _alunoRepository.UnitOfWork.Commit();

            Notificator.Handle("O aluno possui livro(s) não devolvido(s) e atrasado(s). " +
                               "O aluno será impedido de fazer empréstimos/renovações até devolvê-lo(s).");

            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaRealizarEntrega(int id, RealizarEntregaDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return false;
        }

        var emprestimo = await _emprestimoRepository.ObterPorId(id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (emprestimo.StatusEmprestimo is EStatusEmprestimo.Entregue or EStatusEmprestimo.EntregueComAtraso)
        {
            Notificator.Handle("O empréstimo já está como entregue.");
            return false;
        }

        var alunoRegistradoNoEmprestimo = await _alunoRepository.FirstOrDefault(a =>
            a.Matricula == emprestimo.Aluno.Matricula);
        var alunoInformadoNaDto = await _alunoRepository.FirstOrDefault(a =>
            a.Matricula == dto.AlunoMatricula);

        if (alunoInformadoNaDto == null)
        {
            Notificator.Handle("Aluno não encontrado com a matrícula informada.");
            return false;
        }

        if (alunoRegistradoNoEmprestimo!.Matricula != alunoInformadoNaDto.Matricula)
        {
            Notificator.Handle("A matrícula informada não pertence ao aluno que foi registrado para esse empréstimo.");
            return false;
        }

        var livroRegistradoNoEmprestimo = await _livroRepository.FirstOrDefault(l =>
            l.Codigo == emprestimo.Livro.Codigo);
        var livroInformadoNaDto = await _livroRepository.FirstOrDefault(l =>
            l.Codigo == dto.LivroCodigo);

        if (livroInformadoNaDto == null)
        {
            Notificator.Handle("Livro não encontrado com o código informado.");
            return false;
        }

        if (livroRegistradoNoEmprestimo!.Codigo != livroInformadoNaDto.Codigo)
        {
            Notificator.Handle("O código do livro informado não pertence ao livro que foi registrado para esse empréstimo.");
            return false;
        }

        return true;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _emprestimoRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}