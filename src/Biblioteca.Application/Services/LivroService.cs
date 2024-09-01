﻿using AutoMapper;
using Biblioteca.Application.Configurations;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Enums;
using Biblioteca.Domain.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Biblioteca.Application.Services;

public class LivroService : BaseService, ILivroService
{
    private readonly ILivroRepository _livroRepository;
    private readonly string _imageFolderPath;

    public LivroService(INotificator notificator, IMapper mapper, ILivroRepository livroRepository,
        IOptions<StorageSettings> storageSettings) : base(notificator, mapper)
    {
        _livroRepository = livroRepository;
        _imageFolderPath = storageSettings.Value.ImageFolderPath;
    }

    public async Task<LivroDto?> Adicionar(AdicionarLivroDto dto)
    {
        if (!await ValidacoesParaAdicionarLivro(dto))
            return null;

        var livro = Mapper.Map<Livro>(dto);
        livro.Codigo = await GerarCodigo();
        livro.QuantidadeExemplaresDisponiveisParaEmprestimo = livro.QuantidadeExemplaresDisponiveisEmEstoque;
        livro.StatusLivro = EStatusLivro.Disponivel;
        livro.Ativo = true;
        _livroRepository.Adicionar(livro);
        
        return await CommitChanges() ? Mapper.Map<LivroDto>(livro) : null;
    }

    public async Task<LivroDto?> Atualizar(int id, AtualizarLivroDto dto)
    {
        if (!await ValidacoesParaAtualizarLivro(id, dto))
            return null;

        var livro = await _livroRepository.FirstOrDefault(l => l.Id == id);
        MappingParaAtualizarLivro(livro!, dto);
        _livroRepository.Atualizar(livro!);
        
        return await CommitChanges() ? Mapper.Map<LivroDto>(livro) : null;
    }

    public async Task<LivroDto?> UploadCapa(int id, ICollection<IFormFile>? files)
    {
        var livro = await _livroRepository.FirstOrDefault(l => l.Id == id);
        if (livro == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        if (files == null || files.Count == 0)
        {
            Notificator.Handle("Nenhum arquivo enviado.");
            return null;
        }

        foreach (var file in files)
        {
            if (!EhImagem(file))
            {
                Notificator.Handle("Apenas arquivos de imagem são permitidos.");
                return null;
            }

            if (!string.IsNullOrEmpty(livro.NomeArquivoCapa))
            {
                var caminhoImagemAnterior = Path.Combine(_imageFolderPath, livro.NomeArquivoCapa);
                if (File.Exists(caminhoImagemAnterior))
                    File.Delete(caminhoImagemAnterior);
            }

            var nomeArquivo = DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName);
            var caminhoCompleto = Path.Combine(_imageFolderPath, nomeArquivo);

            await using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            livro.NomeArquivoCapa = nomeArquivo;
            _livroRepository.Atualizar(livro);
        }

        return await CommitChanges() ? Mapper.Map<LivroDto>(livro) : null;
    }

    public async Task<PaginacaoDto<LivroDto>> Pesquisar(PesquisarLivroDto dto)
    {
        var resultadoPaginado = await _livroRepository.Pesquisar(dto.Id, dto.Titulo, dto.Autor,
            dto.Editora, dto.Categoria, dto.Codigo, dto.Ativo, dto.QuantidadeDeItensPorPagina, dto.PaginaAtual);

        return new PaginacaoDto<LivroDto>
        {
            TotalDeItens = resultadoPaginado.TotalDeItens,
            QuantidadeDeItensPorPagina = resultadoPaginado.QuantidadeDeItensPorPagina,
            QuantidadeDePaginas = resultadoPaginado.QuantidadeDePaginas,
            PaginaAtual = resultadoPaginado.PaginaAtual,
            Itens = Mapper.Map<List<LivroDto>>(resultadoPaginado.Itens)
        };
    }

    public async Task<List<LivroDto>> ObterTodos()
    {
        var livros = await _livroRepository.ObterTodos();
        return Mapper.Map<List<LivroDto>>(livros);
    }

    public async Task Ativar(int id)
    {
        var livro = await _livroRepository.FirstOrDefault(l => l.Id == id);
        if (livro == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (livro.Ativo)
        {
            Notificator.Handle("O livro informado já está ativado.");
            return;
        }

        livro.Ativo = true;
        _livroRepository.Atualizar(livro);
        
        await CommitChanges();
    }

    public async Task Desativar(int id)
    {
        var livro = await _livroRepository.FirstOrDefault(l => l.Id == id);
        if (livro == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (livro.Ativo == false)
        {
            Notificator.Handle("O livro informado já está desativado.");
            return;
        }

        if (livro.QuantidadeExemplaresDisponiveisParaEmprestimo != livro.QuantidadeExemplaresDisponiveisEmEstoque)
        {
            Notificator.Handle(
                "O livro informado não pode ser desativado, pois o mesmo possui exemplares emprestados.");
            return;
        }

        livro.Ativo = false;
        _livroRepository.Atualizar(livro);
        
        await CommitChanges();
    }

    private async Task<bool> ValidacoesParaAdicionarLivro(AdicionarLivroDto dto)
    {
        var livro = Mapper.Map<Livro>(dto);
        var validador = new LivroValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(livro);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        var livroIgual = await _livroRepository.FirstOrDefault(l =>
            l.Titulo == dto.Titulo &&
            l.Autor == dto.Autor &&
            l.Edicao == dto.Edicao &&
            l.Editora == dto.Editora);
        if (livroIgual != null)
        {
            Notificator.Handle("Já existe um livro cadastrado com o título, autor, edição e editora informados.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaAtualizarLivro(int id, AtualizarLivroDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return false;
        }

        var livroExistente = await _livroRepository.FirstOrDefault(l => l.Id == id);
        if (livroExistente == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (livroExistente.Ativo == false)
        {
            Notificator.Handle("Não é possível atualizar um livro que está desativado.");
            return false;
        }

        if (livroExistente.QuantidadeExemplaresDisponiveisParaEmprestimo <
            livroExistente.QuantidadeExemplaresDisponiveisEmEstoque)
        {
            Notificator.Handle("Não é possível atualizar um livro que tenha algum exemplar emprestado ou renovado.");
            return false;
        }

        var livro = Mapper.Map<Livro>(dto);
        var validador = new LivroValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(livro);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        return true;
    }

    private void MappingParaAtualizarLivro(Livro livro, AtualizarLivroDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Titulo))
            livro.Titulo = dto.Titulo;

        if (!string.IsNullOrEmpty(dto.Autor))
            livro.Autor = dto.Autor;

        if (!string.IsNullOrEmpty(dto.Edicao))
            livro.Edicao = dto.Edicao;

        if (!string.IsNullOrEmpty(dto.Editora))
            livro.Editora = dto.Editora;

        if (!string.IsNullOrEmpty(dto.Categoria))
            livro.Categoria = dto.Categoria;

        if (dto.AnoPublicacao.HasValue)
            livro.AnoPublicacao = (int)dto.AnoPublicacao;

        if (dto.QuantidadeExemplaresDisponiveisEmEstoque.HasValue)
        {
            livro.QuantidadeExemplaresDisponiveisEmEstoque = (int)dto.QuantidadeExemplaresDisponiveisEmEstoque;
            livro.QuantidadeExemplaresDisponiveisParaEmprestimo = livro.QuantidadeExemplaresDisponiveisEmEstoque;
        }
    }

    private bool EhImagem(IFormFile file)
    {
        var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
        var extensao = Path.GetExtension(file.FileName).ToLowerInvariant();

        return extensoesPermitidas.Contains(extensao);
    }

    private async Task<int> GerarCodigo()
    {
        var ultimoCodigo = await _livroRepository.Queryable()
            .OrderByDescending(l => l.Codigo)
            .Select(l => l.Codigo)
            .FirstOrDefaultAsync();

        return ultimoCodigo == 0 ? 1000 : ultimoCodigo + 1;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _livroRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}