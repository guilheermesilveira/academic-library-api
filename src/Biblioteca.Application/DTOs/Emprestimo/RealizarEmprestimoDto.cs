namespace Biblioteca.Application.DTOs.Emprestimo;

public class RealizarEmprestimoDto
{
    public string AlunoMatricula { get; set; } = null!;
    public string AlunoSenha { get; set; } = null!;
    public int LivroCodigo { get; set; }
}