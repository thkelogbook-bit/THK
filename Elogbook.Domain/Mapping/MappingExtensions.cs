using Elogbook.Core.Models.Response.User;
using Elogbook.Data;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Mapping
{
    /// <summary>
    /// Mapping Models and Entity objects
    /// </summary>
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        public static TDestination MapTo<TDestination>(object source)
        {
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }

        #region Main Category

        public static UserModel ToModel(this User entity)
        {
            return entity.MapTo<User, UserModel>();
        }

        public static IEnumerable<UserModel> ToModels(this IEnumerable<User> entities)
        {
            return entities.Select(x => x.MapTo<User, UserModel>());
        }

        public static User ToEntity(this UserModel model)
        {
            return model.MapTo<UserModel, User>();
        }

        #endregion
    }
}