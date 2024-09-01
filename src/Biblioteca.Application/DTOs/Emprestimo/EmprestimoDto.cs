using Biblioteca.Application.DTOs.Aluno;
using Biblioteca.Application.DTOs.Livro;

namespace Biblioteca.Application.DTOs.Emprestimo;

public class EmprestimoDto
{
    public int Id { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoRealizada { get; set; }
    public string StatusEmprestimo { get; set; } = null!;
    public int QuantidadeRenovacoesPermitida { get; set; }
    public int QuantidadeRenovacoesRealizadas { get; set; }
    public bool Ativo { get; set; }
    public AlunoDto Aluno { get; set; } = null!;
    public LivroDto Livro { get; set; } = null!;
}