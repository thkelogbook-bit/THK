namespace Elogbook.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Attachment", "FileType", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.Attachment", "FileType", c => c.String(maxLength: 50));
        }
    }
}
