using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Aluno;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Validators;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Application.Services;

public class AlunoService : BaseService, IAlunoService
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IPasswordHasher<Aluno> _passwordHasher;

    public AlunoService(INotificator notificator, IMapper mapper, IAlunoRepository alunoRepository,
        IPasswordHasher<Aluno> passwordHasher) : base(notificator, mapper)
    {
        _alunoRepository = alunoRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AlunoDto?> Adicionar(AdicionarAlunoDto dto)
    {
        if (!await ValidacoesParaAdicionarAluno(dto))
            return null;

        var aluno = Mapper.Map<Aluno>(dto);
        aluno.Senha = _passwordHasher.HashPassword(aluno, dto.Senha);
        aluno.QuantidadeEmprestimosPermitida = 5;
        aluno.QuantidadeEmprestimosRealizados = 0;
        aluno.Bloqueado = false;
        aluno.Ativo = true;
        _alunoRepository.Adicionar(aluno);
        
        return await CommitChanges() ? Mapper.Map<AlunoDto>(aluno) : null;
    }

    public async Task<AlunoDto?> Atualizar(int id, AtualizarAlunoDto dto)
    {
        if (!await ValidacoesParaAtualizarAluno(id, dto))
            return null;

        var aluno = await _alunoRepository.FirstOrDefault(a => a.Id == id);
        MappingParaAtualizarAluno(aluno!, dto);
        _alunoRepository.Atualizar(aluno!);
        
        return await CommitChanges() ? Mapper.Map<AlunoDto>(aluno) : null;
    }

    public async Task<PaginacaoDto<AlunoDto>> Pesquisar(PesquisarAlunoDto dto)
    {
        var resultadoPaginado = await _alunoRepository.Pesquisar(dto.Id, dto.Nome, dto.Email, dto.Matricula,
            dto.Curso, dto.Ativo, dto.QuantidadeDeItensPorPagina, dto.PaginaAtual);

        return new PaginacaoDto<AlunoDto>
        {
            TotalDeItens = resultadoPaginado.TotalDeItens,
            QuantidadeDeItensPorPagina = resultadoPaginado.QuantidadeDeItensPorPagina,
            QuantidadeDePaginas = resultadoPaginado.QuantidadeDePaginas,
            PaginaAtual = resultadoPaginado.PaginaAtual,
            Itens = Mapper.Map<List<AlunoDto>>(resultadoPaginado.Itens)
        };
    }

    public async Task<List<AlunoDto>> ObterTodos()
    {
        var alunos = await _alunoRepository.ObterTodos();
        return Mapper.Map<List<AlunoDto>>(alunos);
    }

    public async Task Ativar(int id)
    {
        var aluno = await _alunoRepository.FirstOrDefault(a => a.Id == id);
        if (aluno == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (aluno.Ativo)
        {
            Notificator.Handle("O aluno informado já está ativado.");
            return;
        }

        aluno.Ativo = true;
        _alunoRepository.Atualizar(aluno);
        
        await CommitChanges();
    }

    public async Task Desativar(int id)
    {
        var aluno = await _alunoRepository.FirstOrDefault(a => a.Id == id);
        if (aluno == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (aluno.Ativo == false)
        {
            Notificator.Handle("O aluno informado já está desativado.");
            return;
        }

        if (aluno.QuantidadeEmprestimosRealizados > 0)
        {
            Notificator.Handle("O aluno informado não pode ser desativado, pois o mesmo possui livros emprestados.");
            return;
        }

        aluno.Ativo = false;
        _alunoRepository.Atualizar(aluno);
        
        await CommitChanges();
    }

    private async Task<bool> ValidacoesParaAdicionarAluno(AdicionarAlunoDto dto)
    {
        var aluno = Mapper.Map<Aluno>(dto);
        var validador = new AlunoValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(aluno);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        var alunoComMatriculaExistente = await _alunoRepository.FirstOrDefault(a => a.Matricula == dto.Matricula);
        if (alunoComMatriculaExistente != null)
        {
            Notificator.Handle("Já existe um aluno cadastrado com a matrícula informada.");
            return false;
        }

        var alunoComEmailExistente = await _alunoRepository.FirstOrDefault(a => a.Email == dto.Email);
        if (alunoComEmailExistente != null)
        {
            Notificator.Handle("Já existe um aluno cadastrado com o email informado.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaAtualizarAluno(int id, AtualizarAlunoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return false;
        }

        var alunoExistente = await _alunoRepository.FirstOrDefault(a => a.Id == id);
        if (alunoExistente == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (alunoExistente.Ativo == false)
        {
            Notificator.Handle("Não é possível atualizar um aluno que está desativado.");
            return false;
        }

        var aluno = Mapper.Map<Aluno>(dto);
        var validador = new AlunoValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(aluno);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        return true;
    }

    private void MappingParaAtualizarAluno(Aluno aluno, AtualizarAlunoDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Nome))
            aluno.Nome = dto.Nome;

        if (!string.IsNullOrEmpty(dto.Matricula))
            aluno.Matricula = dto.Matricula;

        if (!string.IsNullOrEmpty(dto.Curso))
            aluno.Curso = dto.Curso;

        if (!string.IsNullOrEmpty(dto.Email))
            aluno.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.Senha))
        {
            aluno.Senha = dto.Senha;
            aluno.Senha = _passwordHasher.HashPassword(aluno, dto.Senha);
        }
    }

    private async Task<bool> CommitChanges()
    {
        if (await _alunoRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}