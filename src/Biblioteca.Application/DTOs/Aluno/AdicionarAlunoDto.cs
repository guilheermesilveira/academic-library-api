namespace Biblioteca.Application.DTOs.Aluno;

public class AdicionarAlunoDto
{
    public string Nome { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    public string Curso { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}