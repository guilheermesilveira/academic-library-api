namespace Library.Application.DTOs.Student;

public class UpdateStudentDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Registration { get; set; }
    public string? Course { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}