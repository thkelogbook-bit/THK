using AutoMapper;
using Elogbook.Core.Mapper;
using Elogbook.Core.Models.Response.User;
using Elogbook.Data;

namespace Elogbook.Domain.Mapping
{
    public class ElogbookMapperConfiguration : Profile
    {
        public ElogbookMapperConfiguration()
        {
            // Call function IgnoreNoMap to ignore properties define in entity (add attribute [MapperIgnore] above each property) to response model
            CreateMap<User, UserModel>().IgnoreNoMap();
            CreateMap<UserModel, User>().IgnoreNoMap();
            CreateMap<User, CurrentUserModel>();
        }
    }
}
