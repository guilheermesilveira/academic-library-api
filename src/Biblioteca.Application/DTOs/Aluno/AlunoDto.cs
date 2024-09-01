namespace Biblioteca.Application.DTOs.Aluno;

public class AlunoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    public string Curso { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int QuantidadeEmprestimosPermitida { get; set; }
    public int QuantidadeEmprestimosRealizados { get; set; }
    public bool Bloqueado { get; set; }
    public bool Ativo { get; set; }
}