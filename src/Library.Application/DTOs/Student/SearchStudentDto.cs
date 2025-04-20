namespace Library.Application.DTOs.Student;

public class SearchStudentDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Registration { get; set; }
    public string? Course { get; set; }
    public bool? Active { get; set; }
    public int NumberOfItemsPerPage { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;
}