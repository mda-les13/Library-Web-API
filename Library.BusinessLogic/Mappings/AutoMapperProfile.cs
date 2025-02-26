using AutoMapper;
using Library.BusinessLogic.Models;
using Library.DataAccess.Entities;

namespace Library.BusinessLogic.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Book, BookModel>().ReverseMap();
            CreateMap<Author, AuthorModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<RegisterUserModel, User>().ReverseMap();
        }
    }
}
