namespace Biblioteca.Application.DTOs.Aluno;

public class AtualizarAlunoDto
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Matricula { get; set; }
    public string? Curso { get; set; }
    public string? Email { get; set; }
    public string? Senha { get; set; }
}