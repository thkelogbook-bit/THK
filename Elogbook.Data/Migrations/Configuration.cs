using System.Data.Entity.Migrations;

namespace Elogbook.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ElogbookDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        /// <summary>
        /// Data initilize
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(ElogbookDataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
