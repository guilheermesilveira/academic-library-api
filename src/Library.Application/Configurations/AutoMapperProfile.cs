using AutoMapper;
using Library.Domain.Entities;
using Library.Application.DTOs.Auth;
using Library.Application.DTOs.Book;
using Library.Application.DTOs.Loan;
using Library.Application.DTOs.Student;

namespace Library.Application.Configurations;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Auth

        CreateMap<LoginDto, Administrator>();

        #endregion

        #region Student

        CreateMap<Student, StudentDto>();
        CreateMap<AddStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>();

        #endregion

        #region Book

        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.BookStatus, opt => opt.MapFrom(src => src.BookStatus.ToString()));
        CreateMap<AddBookDto, Book>();
        CreateMap<UpdateBookDto, Book>();

        #endregion

        #region Loan

        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.LoanStatus, opt => opt.MapFrom(src => src.LoanStatus.ToString()))
            .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Student))
            .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));

        #endregion
    }
}