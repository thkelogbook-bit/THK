using Core.System.Runtime.Managers;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Elogbook.Data
{
    public partial class PmisSyncDataContext : DbContext
    {
        public PmisSyncDataContext()
            : base(RuntimeManager.GetConnectionString("PmisSyncConnection"))
        {
            int.TryParse(ConfigurationManager.AppSettings["EFCommandTimeout"], out int commandTimeout);
            if (commandTimeout == 0) commandTimeout = 180;
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = commandTimeout;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
