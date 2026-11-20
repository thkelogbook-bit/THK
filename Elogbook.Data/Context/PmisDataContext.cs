using Core.System.Runtime.Managers;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Elogbook.Data
{
    public partial class PmisDataContext : DbContext
    {
        public PmisDataContext()
            : base(RuntimeManager.GetConnectionString("PmisConnection"))
        {
            int.TryParse(ConfigurationManager.AppSettings["EFCommandTimeout"], out int commandTimeout);
            if (commandTimeout == 0) commandTimeout = 180;
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = commandTimeout;
        }

        public virtual DbSet<A_ASSET_METER_DYN> A_ASSET_METER_DYN { get; set; }

        public virtual DbSet<A_METER> A_METER { get; set; }

        public virtual DbSet<OP_METER_TAB> OP_METER_TAB { get; set; }

        public virtual DbSet<OP_METER_TAB_DT> OP_METER_TAB_DT { get; set; }

#pragma warning disable IDE1006 // Naming Styles
        public virtual DbSet<S_ORGANIZATION> S_ORGANIZATION { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public virtual DbSet<A_LST_TYPE> A_LST_TYPE { get; set; }

        public virtual DbSet<A_LST_CATEGORY> A_LST_CATEGORY { get; set; }

        public virtual DbSet<A_ASSET> A_ASSET { get; set; }

        public virtual DbSet<G_ERP_COUNTRY> G_ERP_COUNTRY { get; set; }

#pragma warning disable IDE1006 // Naming Styles
        public virtual DbSet<S_COMPANY> S_COMPANY { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public virtual DbSet<S_SITE> S_SITE { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
