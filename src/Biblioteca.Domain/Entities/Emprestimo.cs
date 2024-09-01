using Biblioteca.Domain.Enums;

namespace Biblioteca.Domain.Entities;

public class Emprestimo : Entity
{
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoRealizada { get; set; }
    public EStatusEmprestimo StatusEmprestimo { get; set; }
    public int QuantidadeRenovacoesPermitida { get; set; }
    public int QuantidadeRenovacoesRealizadas { get; set; }
    public int AlunoId { get; set; }
    public int LivroId { get; set; }

    // Relation
    public Aluno Aluno { get; set; } = null!;
    public Livro Livro { get; set; } = null!;
}