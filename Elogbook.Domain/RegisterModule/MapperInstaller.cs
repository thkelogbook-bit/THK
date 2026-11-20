using AutoMapper;
using Elogbook.Domain.Mapping;

namespace Elogbook.Domain.RegisterModule
{
    public static class MapperInstaller
    {
        public static void Init()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(ElogbookMapperConfiguration));
            });

            //register
            AutoMapperConfiguration.Init(config);
        }
    }
}
