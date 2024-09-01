namespace Biblioteca.Application.DTOs.Emprestimo;

public class PesquisarEmprestimoDto
{
    public int? Id { get; set; }
    public int? AlunoId { get; set; }
    public string? AlunoMatricula { get; set; }
    public int? LivroId { get; set; }
    public int? LivroCodigo { get; set; }
    public bool? Ativo { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; } = 10;
    public int PaginaAtual { get; set; } = 1;
}