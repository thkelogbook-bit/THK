namespace Elogbook.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AppVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppVersion",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Version = c.String(nullable: false, maxLength: 50),
                    ForceUpdate = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropTable("dbo.AppVersion");
        }
    }
}
