using Autofac;

namespace Elogbook.Data.RegisterModule
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register data context
            builder.RegisterType(typeof(ElogbookDataContext))
                .InstancePerLifetimeScope();

            // Register unit of work
            builder.RegisterType(typeof(ElogbookUnitOfWork))
                .InstancePerLifetimeScope();

            // Register data context
            builder.RegisterType(typeof(PmisDataContext))
                .InstancePerLifetimeScope();

            // Register unit of work
            builder.RegisterType(typeof(PmisUnitOfWork))
                .InstancePerLifetimeScope();

            // Register data context
            builder.RegisterType(typeof(PmisSyncDataContext))
                .InstancePerLifetimeScope();

            // Register unit of work
            builder.RegisterType(typeof(PmisSyncUnitOfWork))
                .InstancePerLifetimeScope();
        }
    }
}
