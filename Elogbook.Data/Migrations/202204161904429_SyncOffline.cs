namespace Elogbook.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SyncOffline : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.OperationParameters");
            AddColumn("dbo.AbnormalIncident", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.DeliveryReceiveShift", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.OperationDiary", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.DeliveryShiftMethod", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.ExistInShift", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.OperationDiaryPositionShift", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.OperationSituation", "SyncDataKey", c => c.String(maxLength: 100));
            AddColumn("dbo.OperationParameters", "SyncDataKey", c => c.String(maxLength: 100));
            AlterColumn("dbo.OperationParameters", "ID", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.OperationParameters", "ID");
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.OperationParameters");
            AlterColumn("dbo.OperationParameters", "ID", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.OperationParameters", "SyncDataKey");
            DropColumn("dbo.OperationSituation", "SyncDataKey");
            DropColumn("dbo.OperationDiaryPositionShift", "SyncDataKey");
            DropColumn("dbo.ExistInShift", "SyncDataKey");
            DropColumn("dbo.DeliveryShiftMethod", "SyncDataKey");
            DropColumn("dbo.OperationDiary", "SyncDataKey");
            DropColumn("dbo.DeliveryReceiveShift", "SyncDataKey");
            DropColumn("dbo.AbnormalIncident", "SyncDataKey");
            AddPrimaryKey("dbo.OperationParameters", "ID");
        }
    }
}
