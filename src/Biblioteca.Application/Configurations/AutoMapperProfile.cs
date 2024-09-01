using AutoMapper;
using Biblioteca.Application.DTOs.Aluno;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Configurations;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Auth

        CreateMap<LoginDto, Administrador>();

        #endregion

        #region Aluno

        CreateMap<Aluno, AlunoDto>();
        CreateMap<AdicionarAlunoDto, Aluno>();
        CreateMap<AtualizarAlunoDto, Aluno>();

        #endregion

        #region Livro

        CreateMap<Livro, LivroDto>()
            .ForMember(dest => dest.StatusLivro, opt => opt.MapFrom(src => src.StatusLivro.ToString()));
        CreateMap<AdicionarLivroDto, Livro>();
        CreateMap<AtualizarLivroDto, Livro>();

        #endregion

        #region Emprestimo

        CreateMap<Emprestimo, EmprestimoDto>()
            .ForMember(dest => dest.StatusEmprestimo, opt => opt.MapFrom(src => src.StatusEmprestimo.ToString()))
            .ForMember(dest => dest.Aluno, opt => opt.MapFrom(src => src.Aluno))
            .ForMember(dest => dest.Livro, opt => opt.MapFrom(src => src.Livro));

        #endregion
    }
}