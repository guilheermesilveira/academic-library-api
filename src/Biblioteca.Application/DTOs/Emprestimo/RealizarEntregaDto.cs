namespace Biblioteca.Application.DTOs.Emprestimo;

public class RealizarEntregaDto
{
    public int Id { get; set; }
    public string AlunoMatricula { get; set; } = null!;
    public int LivroCodigo { get; set; }
}