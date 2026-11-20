namespace Elogbook.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AbnormalIncident",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationDiaryID = c.Int(nullable: false),
                    Description = c.String(),
                    Attachments = c.String(maxLength: 500),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FullName = c.String(nullable: false, maxLength: 255),
                    UserName = c.String(nullable: false, maxLength: 255),
                    Password = c.String(maxLength: 100),
                    PositionName = c.String(maxLength: 255),
                    DateOfBirth = c.DateTime(),
                    PhoneNumber = c.String(maxLength: 30),
                    Email = c.String(maxLength: 255),
                    NationalID = c.String(maxLength: 50),
                    NationalAttachmentID = c.Int(nullable: false),
                    ElectricRank = c.Int(),
                    ElectricRankCard = c.String(maxLength: 255),
                    ElectricRankAttachmentID = c.Int(nullable: false),
                    LaborCard = c.String(maxLength: 255),
                    LaborRank = c.Int(nullable: false),
                    LaborCardAttachmentID = c.Int(nullable: false),
                    Signature = c.String(),
                    CompanyID = c.Int(nullable: false),
                    ProductionPlantID = c.Int(),
                    TeamID = c.Int(),
                    GroupID = c.Int(),
                    FactoryID = c.Int(),
                    AccountType = c.Int(nullable: false),
                    SourceType = c.Int(nullable: false),
                    Status = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Organization", t => t.CompanyID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Organization", t => t.GroupID)
                .ForeignKey("dbo.Organization", t => t.ProductionPlantID)
                .ForeignKey("dbo.Organization", t => t.TeamID)
                .Index(t => t.CompanyID)
                .Index(t => t.ProductionPlantID)
                .Index(t => t.TeamID)
                .Index(t => t.GroupID)
                .Index(t => t.FactoryID);

            CreateTable(
                "dbo.Organization",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ParentID = c.Int(),
                    Name = c.String(nullable: false, maxLength: 255),
                    ShortName = c.String(nullable: false, maxLength: 100),
                    Level = c.String(nullable: false, maxLength: 100),
                    CompanyType = c.String(maxLength: 255),
                    ProductionPlantType = c.String(maxLength: 100),
                    Active = c.Int(nullable: false),
                    AttachmentID = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Attachment", t => t.AttachmentID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.Organization", t => t.ParentID)
                .Index(t => t.ParentID)
                .Index(t => t.AttachmentID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Attachment",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FilePath = c.String(maxLength: 500),
                    PathThumbnail = c.String(maxLength: 500),
                    FileName = c.String(maxLength: 500),
                    FileType = c.String(maxLength: 50),
                    Description = c.String(),
                    ScreenName = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Factory",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FactoryName = c.String(nullable: false, maxLength: 255),
                    CompanyID = c.Int(nullable: false),
                    ProductionPlantID = c.Int(nullable: false),
                    Status = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Organization", t => t.CompanyID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.Organization", t => t.ProductionPlantID)
                .Index(t => t.CompanyID)
                .Index(t => t.ProductionPlantID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.RoleInUsers",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    UserRoleID = c.Int(nullable: false),
                    UserID = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.UserRoles", t => t.UserRoleID, cascadeDelete: true)
                .Index(t => t.UserRoleID)
                .Index(t => t.UserID);

            CreateTable(
                "dbo.UserRoles",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RoleName = c.String(nullable: false, maxLength: 255),
                    Description = c.String(),
                    Active = c.Int(nullable: false),
                    DataPermission = c.Int(nullable: false),
                    Permissions = c.String(),
                    IsAdmin = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.AlarmSettings",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    SettingName = c.String(nullable: false, maxLength: 100),
                    SettingType = c.Int(nullable: false),
                    ColorCode = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.AlertDetails",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ParameterID = c.Int(),
                    Alert = c.String(maxLength: 100),
                    Operator = c.String(maxLength: 100),
                    Value = c.Single(nullable: false),
                    Color = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.Parameters", t => t.ParameterID)
                .Index(t => t.ParameterID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Parameters",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    IsGroup = c.Boolean(nullable: false),
                    FactoryID = c.Int(),
                    OperationPositionID = c.Int(),
                    CycleID = c.Int(),
                    ParameterGroupID = c.Int(),
                    RFIDCode = c.String(maxLength: 255),
                    QRCode = c.String(maxLength: 255),
                    ParameterName = c.String(nullable: false, maxLength: 255),
                    KKSCode = c.String(maxLength: 255),
                    UnitMeasure = c.String(nullable: false, maxLength: 255),
                    Setting = c.String(maxLength: 255),
                    FromRange = c.Single(),
                    ToRange = c.Single(),
                    IsNumberType = c.Boolean(nullable: false),
                    QLKTCode = c.String(maxLength: 255),
                    Status = c.Int(nullable: false),
                    Position = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.CycleManagement", t => t.CycleID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationPosition", t => t.OperationPositionID)
                .ForeignKey("dbo.ParameterGroup", t => t.ParameterGroupID)
                .Index(t => t.FactoryID)
                .Index(t => t.OperationPositionID)
                .Index(t => t.CycleID)
                .Index(t => t.ParameterGroupID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.CycleManagement",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CycleName = c.String(nullable: false, maxLength: 255),
                    Position = c.Int(nullable: false),
                    TimeLine = c.String(maxLength: 1000),
                    OpenInputBefore = c.Int(nullable: false),
                    BlockInputAfter = c.Int(nullable: false),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.CycleFactory",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CycleID = c.Int(nullable: false),
                    FactoryID = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.CycleManagement", t => t.CycleID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CycleID)
                .Index(t => t.FactoryID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationPosition",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationPositionName = c.String(nullable: false, maxLength: 255),
                    FactoryID = c.Int(),
                    Active = c.Int(nullable: false),
                    Quantity = c.Int(nullable: false),
                    AuthenticationType = c.Int(nullable: false),
                    IsUsingOnlyOneTablet = c.Boolean(nullable: false),
                    AllowWriteDiaryPortal = c.Boolean(nullable: false),
                    AllowWriteDiaryMobile = c.Boolean(nullable: false),
                    AllowWriteParameterPortal = c.Boolean(nullable: false),
                    AllowWriteParameterMobile = c.Boolean(nullable: false),
                    JobTitle = c.Int(nullable: false),
                    TemplateDeliveryShiftMethod = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.FactoryID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.PositionInShift",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ParentID = c.Int(nullable: false),
                    OperationPositionID = c.Int(nullable: false),
                    IsSystem = c.Boolean(nullable: false),
                    Active = c.Int(nullable: false),
                    Quantity = c.Int(nullable: false),
                    Position = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationPosition", t => t.OperationPositionID, cascadeDelete: true)
                .Index(t => t.OperationPositionID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ParameterGroup",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ParameterGroupName = c.String(nullable: false, maxLength: 255),
                    FactoryID = c.Int(),
                    OperationPositionID = c.Int(),
                    CycleID = c.Int(),
                    Position = c.Int(nullable: false),
                    Status = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.CycleManagement", t => t.CycleID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationPosition", t => t.OperationPositionID)
                .Index(t => t.FactoryID)
                .Index(t => t.OperationPositionID)
                .Index(t => t.CycleID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.AllowedInfo",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(),
                    AllowedTime = c.DateTime(nullable: false),
                    CommanderSignature = c.Int(),
                    TimeCommanderSigned = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID)
                .Index(t => t.WorkTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.AllowedInfoDetail",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    AllowedInforID = c.Int(nullable: false),
                    Content = c.String(),
                    Position = c.Int(),
                    Note = c.String(),
                    IsChecked = c.Boolean(),
                    Attachments = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AllowedInfo", t => t.AllowedInforID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.AllowedInforID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkTicket",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ScreenType = c.Int(),
                    WorkTicketNumber = c.String(nullable: false, maxLength: 100),
                    RegistrationTicketID = c.Int(),
                    FactoryID = c.Int(),
                    ApprovedUnitID = c.Int(),
                    ApprovedDate = c.DateTime(nullable: false),
                    WorkTicketType = c.Int(nullable: false),
                    WorkTicketStatus = c.Int(nullable: false),
                    UnitNumber = c.Int(),
                    WorkPlacement = c.String(),
                    WorkContent = c.String(),
                    WorkScope = c.String(),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                    WorkCondition = c.String(),
                    Attachments = c.String(maxLength: 500),
                    WorkPictures = c.String(),
                    WorkResult = c.String(),
                    WorkTicketFeatureID = c.Int(),
                    IsFlagNote = c.Boolean(nullable: false),
                    TicketPdfID = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Organization", t => t.ApprovedUnitID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationTicket", t => t.RegistrationTicketID)
                .ForeignKey("dbo.TicketStatus", t => t.WorkTicketStatus, cascadeDelete: true)
                .ForeignKey("dbo.WorkTicketFeatures", t => t.WorkTicketFeatureID)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.FactoryID)
                .Index(t => t.ApprovedUnitID)
                .Index(t => t.WorkTicketStatus)
                .Index(t => t.WorkTicketFeatureID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.LeadershipUser",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(nullable: false),
                    UserName = c.String(maxLength: 100),
                    FullName = c.String(maxLength: 100),
                    PositionName = c.String(maxLength: 500),
                    Roles = c.Int(),
                    ElectricRank = c.Int(),
                    ElectricRankCard = c.String(maxLength: 255),
                    Signature = c.String(),
                    IsExternal = c.Boolean(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID, cascadeDelete: true)
                .Index(t => t.WorkTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.RegistrationTicket",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketType = c.Int(nullable: false),
                    FactoryID = c.Int(nullable: false),
                    TicketNumber = c.String(nullable: false, maxLength: 255),
                    RegistrationDate = c.DateTime(),
                    ReceiveUnitID = c.Int(nullable: false),
                    RegulationNumber = c.String(nullable: false, maxLength: 255),
                    RegistrationUnitID = c.Int(),
                    RegistrationUnitView = c.String(maxLength: 500),
                    Position = c.String(maxLength: 500),
                    PhoneNumber = c.String(maxLength: 20),
                    Email = c.String(maxLength: 255),
                    WorkContent = c.String(nullable: false),
                    WorkPlacement = c.String(nullable: false),
                    WorkCondition = c.String(nullable: false),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                    UnitNumberAndUnitStaff = c.String(maxLength: 255),
                    Units = c.String(),
                    StatusValue = c.Int(nullable: false),
                    Attachments = c.String(maxLength: 500),
                    LocationDevice = c.String(),
                    Reason = c.Int(nullable: false),
                    ContentType = c.Int(nullable: false),
                    IsWorkDaily = c.Boolean(nullable: false),
                    StartTimeDaily = c.String(maxLength: 100),
                    EndTimeDaily = c.String(maxLength: 100),
                    OtherContents = c.String(),
                    IsCollaborateTicket = c.Boolean(nullable: false),
                    RegistrationTicketType = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.Organization", t => t.ReceiveUnitID)
                .ForeignKey("dbo.Organization", t => t.RegistrationUnitID)
                .ForeignKey("dbo.TicketStatus", t => t.StatusValue, cascadeDelete: true)
                .Index(t => t.FactoryID)
                .Index(t => t.ReceiveUnitID)
                .Index(t => t.RegistrationUnitID)
                .Index(t => t.StatusValue)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ContentPosition",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RegistrationTicketID = c.Int(nullable: false),
                    Position = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationTicket", t => t.RegistrationTicketID, cascadeDelete: true)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Stakeholders",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    AssignToUserID = c.Int(),
                    FullName = c.String(maxLength: 100),
                    IsExternal = c.Boolean(nullable: false),
                    UserName = c.String(maxLength: 100),
                    ElectricRank = c.Int(nullable: false),
                    ElectricRankCard = c.String(maxLength: 255),
                    Roles = c.Int(nullable: false),
                    PositionName = c.String(maxLength: 500),
                    Mission = c.String(),
                    RegistrationTicketID = c.Int(),
                    ContentPositionID = c.Int(),
                    JobRank = c.String(maxLength: 500),
                    Location = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.AssignToUserID)
                .ForeignKey("dbo.ContentPosition", t => t.ContentPositionID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationTicket", t => t.RegistrationTicketID)
                .Index(t => t.AssignToUserID)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.ContentPositionID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.IdentificationAndSafetyMeasures",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RegistrationTicketID = c.Int(nullable: false),
                    Identification = c.String(),
                    SafetyMeasures = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationTicket", t => t.RegistrationTicketID, cascadeDelete: true)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OtherRequirements",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Location = c.String(),
                    SafetyMeasures = c.String(),
                    RegistrationTicketID = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationTicket", t => t.RegistrationTicketID, cascadeDelete: true)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.TicketStatus",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    StatusName = c.String(nullable: false, maxLength: 255),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.TicketStatusHistory",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RegistrationTicketID = c.Int(),
                    RegistrationManipulationID = c.Int(),
                    Description = c.String(nullable: false),
                    Reason = c.String(nullable: false),
                    Attachments = c.String(maxLength: 500),
                    StatusValue = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationManipulationTicket", t => t.RegistrationManipulationID)
                .ForeignKey("dbo.RegistrationTicket", t => t.RegistrationTicketID)
                .ForeignKey("dbo.TicketStatus", t => t.StatusValue, cascadeDelete: true)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.RegistrationManipulationID)
                .Index(t => t.StatusValue)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.RegistrationManipulationTicket",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketTypeValue = c.Int(nullable: false),
                    FactoryID = c.Int(),
                    CreateDate = c.DateTime(nullable: false),
                    ApproveUnitID = c.Int(),
                    ApproveUnitView = c.String(maxLength: 255),
                    TicketName = c.String(maxLength: 500),
                    CreatedUserPositionName = c.String(maxLength: 255),
                    ApprovedUserID = c.Int(nullable: false),
                    ApprovedUserPositionName = c.String(maxLength: 255),
                    Purpose = c.String(nullable: false),
                    StartTime = c.DateTime(nullable: false),
                    EndTime = c.DateTime(nullable: false),
                    RecommendedUnit = c.String(),
                    ReferAttachmentIDs = c.String(maxLength: 500),
                    TicketStatusValue = c.Int(nullable: false),
                    RegistrationDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.ApprovedUserID, cascadeDelete: true)
                .ForeignKey("dbo.Organization", t => t.ApproveUnitID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.TicketManipulationStatus", t => t.TicketStatusValue, cascadeDelete: true)
                .Index(t => t.FactoryID)
                .Index(t => t.ApproveUnitID)
                .Index(t => t.ApprovedUserID)
                .Index(t => t.TicketStatusValue)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.TicketManipulationStatus",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    StatusName = c.String(nullable: false, maxLength: 255),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WarnAndSafe",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(nullable: false),
                    WarningContent = c.String(nullable: false),
                    SafetyMeasureContent = c.String(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID, cascadeDelete: true)
                .Index(t => t.WorkTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkDaily",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(),
                    WorkLocationID = c.Int(),
                    WorkDay = c.DateTime(nullable: false),
                    WorkLocationText = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.ContentPosition", t => t.WorkLocationID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID)
                .Index(t => t.WorkTicketID)
                .Index(t => t.WorkLocationID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.StaffInformation",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkDailyID = c.Int(nullable: false),
                    FullName = c.String(nullable: false, maxLength: 100),
                    UserName = c.String(nullable: false, maxLength: 100),
                    IsExternal = c.Boolean(),
                    Mission = c.String(),
                    PositionName = c.String(maxLength: 255),
                    ElectricRankCard = c.String(),
                    CheckInTime = c.String(maxLength: 100),
                    CheckInSignature = c.Int(),
                    CheckOutTime = c.String(maxLength: 100),
                    CheckOutSignature = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkDaily", t => t.WorkDailyID, cascadeDelete: true)
                .Index(t => t.WorkDailyID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkerInformation",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(nullable: false),
                    FullName = c.String(nullable: false, maxLength: 100),
                    CheckInTime = c.DateTime(nullable: false),
                    CheckInSignature = c.Int(),
                    CheckOutTime = c.DateTime(nullable: false),
                    CheckOutSignature = c.Int(),
                    UserName = c.String(maxLength: 100),
                    IsExternal = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID, cascadeDelete: true)
                .Index(t => t.WorkTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkPlacement",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(nullable: false),
                    Place = c.String(nullable: false),
                    StartTime = c.DateTime(nullable: false),
                    EndTime = c.DateTime(nullable: false),
                    CommanderName = c.String(maxLength: 100),
                    CommanderSignature = c.Int(),
                    AcceptorName = c.String(maxLength: 100),
                    AcceptorSignature = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID, cascadeDelete: true)
                .Index(t => t.WorkTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkTicketFeatures",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FeatureName = c.String(nullable: false, maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.CancelTicketInfo",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketID = c.Int(nullable: false),
                    Reason = c.String(),
                    ReferAttachmentIDs = c.String(maxLength: 500),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.DeliveryAccepDevice",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketID = c.Int(nullable: false),
                    IsBeforeDelivery = c.Boolean(nullable: false),
                    DeliveryTime = c.DateTime(nullable: false),
                    Unit = c.String(maxLength: 500),
                    FullName = c.String(maxLength: 255),
                    Content = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.DeliveryReceiveShift",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationDiaryID = c.Int(nullable: false),
                    OperationDiaryPositionShiftID = c.Int(nullable: false),
                    SupervisorID = c.Int(nullable: false),
                    IsDeliveryShift = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationDiary", t => t.OperationDiaryID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.SupervisorID, cascadeDelete: true)
                .Index(t => t.OperationDiaryID)
                .Index(t => t.SupervisorID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationDiary",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    FactoryID = c.Int(),
                    CreatedDate = c.DateTime(nullable: false),
                    OperationPositionID = c.Int(),
                    OperationShiftID = c.Int(),
                    OperationCrewID = c.Int(),
                    AbnormalIncidentID = c.Int(),
                    DeliveryShiftMethodID = c.Int(),
                    Comments = c.String(),
                    IsReceived = c.Boolean(nullable: false),
                    IsDelivered = c.Boolean(nullable: false),
                    IsCanEdit = c.Boolean(nullable: false),
                    TimeDelivery = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AbnormalIncident", t => t.AbnormalIncidentID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.DeliveryShiftMethod", t => t.DeliveryShiftMethodID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationCrew", t => t.OperationCrewID)
                .ForeignKey("dbo.OperationPosition", t => t.OperationPositionID)
                .ForeignKey("dbo.OperationShift", t => t.OperationShiftID)
                .Index(t => t.FactoryID)
                .Index(t => t.OperationPositionID)
                .Index(t => t.OperationShiftID)
                .Index(t => t.OperationCrewID)
                .Index(t => t.AbnormalIncidentID)
                .Index(t => t.DeliveryShiftMethodID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.DeliveryShiftMethod",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationDiaryID = c.Int(nullable: false),
                    Description = c.String(),
                    Attachments = c.String(maxLength: 500),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ExistInShift",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationDiaryID = c.Int(nullable: false),
                    RegistrationTicketID = c.Int(),
                    RegistrationTicketName = c.String(maxLength: 255),
                    TeamLeader = c.String(maxLength: 255),
                    Area = c.String(),
                    Content = c.String(nullable: false),
                    Attachments = c.String(maxLength: 500),
                    StatusID = c.Int(nullable: false),
                    IsDeleteIssueOfShift = c.Boolean(nullable: false),
                    CompletedDateTime = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationDiary", t => t.OperationDiaryID, cascadeDelete: true)
                .ForeignKey("dbo.WorkTicket", t => t.RegistrationTicketID)
                .Index(t => t.OperationDiaryID)
                .Index(t => t.RegistrationTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.IssueOfShiftDetail",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Content = c.String(),
                    Attachments = c.String(maxLength: 500),
                    IsComplete = c.Int(nullable: false),
                    ExistInShiftID = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.ExistInShift", t => t.ExistInShiftID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.ExistInShiftID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationCrew",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationCrewName = c.String(nullable: false, maxLength: 255),
                    FactoryID = c.Int(),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.FactoryID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationDiaryPositionShift",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationDiaryID = c.Int(nullable: false),
                    PositionInShiftID = c.Int(),
                    Label = c.String(maxLength: 255),
                    AssignedUserID = c.Int(nullable: false),
                    IsDefault = c.Boolean(nullable: false),
                    Position = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.AssignedUserID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationDiary", t => t.OperationDiaryID, cascadeDelete: true)
                .ForeignKey("dbo.PositionInShift", t => t.PositionInShiftID)
                .Index(t => t.OperationDiaryID)
                .Index(t => t.PositionInShiftID)
                .Index(t => t.AssignedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationShift",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationShiftName = c.String(nullable: false, maxLength: 255),
                    Position = c.Int(nullable: false),
                    FactoryID = c.Int(nullable: false),
                    StartTime = c.Double(nullable: false),
                    EndTime = c.Double(nullable: false),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.FactoryID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.UsedTimes",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ActiveTime = c.DateTime(),
                    InactiveTime = c.DateTime(),
                    OperationShiftID = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationShift", t => t.OperationShiftID)
                .Index(t => t.OperationShiftID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationSituation",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    OperationDiaryID = c.Int(nullable: false),
                    Hour = c.Double(nullable: false),
                    AssignedFromID = c.Int(),
                    AssignedFromName = c.String(maxLength: 255),
                    AssignedToID = c.Int(),
                    Description = c.String(nullable: false),
                    Attachments = c.String(maxLength: 500),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.AssignedFromID)
                .ForeignKey("dbo.Users", t => t.AssignedToID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationDiary", t => t.OperationDiaryID, cascadeDelete: true)
                .Index(t => t.OperationDiaryID)
                .Index(t => t.AssignedFromID)
                .Index(t => t.AssignedToID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ExclusiveData",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketNumber = c.Int(nullable: false),
                    TicketType = c.String(maxLength: 50),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.GuestUsers",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    UserName = c.String(maxLength: 255),
                    FullName = c.String(nullable: false, maxLength: 255),
                    DateOfBirth = c.DateTime(),
                    NationalID = c.String(maxLength: 50),
                    NationalAttachmentID = c.Int(),
                    Notes = c.String(),
                    ElectricRankCard = c.String(maxLength: 255),
                    ElectricRank = c.Int(),
                    ElectricRankAttachmentID = c.Int(),
                    LaborCard = c.String(maxLength: 255),
                    LaborRank = c.Int(),
                    LaborCardAttachmentID = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ManipulationTemplateTickets",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TemplateTicketName = c.String(),
                    IdFactorysTemplateTicket = c.String(),
                    FactorysTemplateTicket = c.String(),
                    Status = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ManipulationTicket",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketType = c.Int(nullable: false),
                    RegistrationManipulationTicketID = c.Int(),
                    FactoryID = c.Int(),
                    CreatedDate = c.DateTime(nullable: false),
                    ApproveUnitID = c.Int(),
                    ApproveUnitView = c.String(maxLength: 500),
                    TicketName = c.String(maxLength: 500),
                    TicketNumber = c.String(maxLength: 255),
                    CreatedUserTicket = c.String(maxLength: 100),
                    ApprovedUserTicket = c.String(maxLength: 100),
                    Purpose = c.String(),
                    StartTime = c.DateTime(nullable: false),
                    EndTime = c.DateTime(nullable: false),
                    RecommendedUnit = c.String(),
                    CaseAbnormal = c.String(),
                    WorkEndTime = c.DateTime(),
                    CompletedTime = c.DateTime(),
                    ReferAttachmentIDs = c.String(maxLength: 500),
                    MapAttachmentIDs = c.String(maxLength: 500),
                    TicketStatusValue = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Organization", t => t.ApproveUnitID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.RegistrationManipulationTicket", t => t.RegistrationManipulationTicketID)
                .ForeignKey("dbo.TicketManipulationStatus", t => t.TicketStatusValue, cascadeDelete: true)
                .Index(t => t.RegistrationManipulationTicketID)
                .Index(t => t.FactoryID)
                .Index(t => t.ApproveUnitID)
                .Index(t => t.TicketStatusValue)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Modules",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ModuleValue = c.String(nullable: false, maxLength: 50),
                    ModuleName = c.String(nullable: false, maxLength: 255),
                    SystemType = c.String(nullable: false, maxLength: 50),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationParameters",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ParameterName = c.String(nullable: false, maxLength: 255),
                    ParameterID = c.Int(nullable: false),
                    ParameterGroupID = c.Int(),
                    OperationTimeLineID = c.Int(),
                    FactoryID = c.Int(nullable: false),
                    OperationPositionID = c.Int(),
                    CycleID = c.Int(),
                    CurrentValue = c.String(),
                    KKSCode = c.String(maxLength: 255),
                    UnitMeasure = c.String(nullable: false, maxLength: 255),
                    Setting = c.String(),
                    FromRange = c.Single(),
                    ToRange = c.Single(),
                    IsNumberType = c.Boolean(nullable: false),
                    PreviousValue = c.String(),
                    ErrorCode = c.Int(),
                    IsChecked = c.Boolean(nullable: false),
                    AlertDetails = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.CycleManagement", t => t.CycleID)
                .ForeignKey("dbo.Factory", t => t.FactoryID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationPosition", t => t.OperationPositionID)
                .ForeignKey("dbo.OperationTimeLine", t => t.OperationTimeLineID)
                .ForeignKey("dbo.ParameterGroup", t => t.ParameterGroupID)
                .Index(t => t.ParameterGroupID)
                .Index(t => t.OperationTimeLineID)
                .Index(t => t.FactoryID)
                .Index(t => t.OperationPositionID)
                .Index(t => t.CycleID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OperationTimeLine",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TimeLineSyncDataKey = c.String(maxLength: 100),
                    TimeLineName = c.Int(nullable: false),
                    FactoryID = c.Int(),
                    OperationPositionID = c.Int(),
                    CycleID = c.Int(),
                    TotalParametersDefault = c.Int(nullable: false),
                    TotalParametersChecked = c.Int(nullable: false),
                    TotalParametersSynchronized = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.CycleManagement", t => t.CycleID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OperationPosition", t => t.OperationPositionID)
                .Index(t => t.FactoryID)
                .Index(t => t.OperationPositionID)
                .Index(t => t.CycleID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.OtherRequest",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketID = c.Int(),
                    RegistrationManipulationID = c.Int(),
                    IsCreated = c.Boolean(nullable: false),
                    Description = c.String(nullable: false),
                    Type = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Participant",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketID = c.Int(),
                    RegistrationManipulationID = c.Int(),
                    IsCreated = c.Boolean(nullable: false),
                    UserName = c.String(maxLength: 100),
                    FullName = c.String(maxLength: 100),
                    PositionName = c.String(maxLength: 255),
                    Roles = c.Int(nullable: false),
                    ElectricRank = c.Int(),
                    ElectricRankCard = c.String(maxLength: 255),
                    SignatureID = c.Int(),
                    IsExternal = c.Boolean(),
                    Order = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.PowerUnit",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    PowerUnitName = c.String(nullable: false, maxLength: 255),
                    FactoryID = c.Int(),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Factory", t => t.FactoryID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.FactoryID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.ReceiveWorkplace",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(),
                    WorkUnitName = c.String(maxLength: 255),
                    WorkUnitID = c.Int(),
                    AmountPersons = c.String(maxLength: 255),
                    StartWorkingDate = c.DateTime(nullable: false),
                    LeaderSignature = c.Int(),
                    CommanderSignature = c.Int(),
                    SupervisorSignature = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID)
                .ForeignKey("dbo.Organization", t => t.WorkUnitID)
                .Index(t => t.WorkTicketID)
                .Index(t => t.WorkUnitID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.SafetyMeasure",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ReceiveWorkplaceID = c.Int(),
                    Content = c.String(),
                    Position = c.Int(),
                    Note = c.String(),
                    IsChecked = c.Boolean(),
                    Attachments = c.String(maxLength: 500),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.ReceiveWorkplace", t => t.ReceiveWorkplaceID)
                .Index(t => t.ReceiveWorkplaceID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.SequenceOperation",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketID = c.Int(),
                    RegistrationManipulationID = c.Int(),
                    ManipulationTemplateTicketID = c.Int(),
                    IsCreated = c.Boolean(nullable: false),
                    TableOfContents = c.String(maxLength: 255),
                    Location = c.String(),
                    Step = c.Int(nullable: false),
                    Content = c.String(),
                    BeginTime = c.DateTime(),
                    EndTime = c.DateTime(),
                    LeaderID = c.Int(),
                    WorkerID = c.Int(),
                    IsDone = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.SequenceOperationParticipant", t => t.LeaderID)
                .ForeignKey("dbo.SequenceOperationParticipant", t => t.WorkerID)
                .Index(t => t.LeaderID)
                .Index(t => t.WorkerID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.SequenceOperationParticipant",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    TicketID = c.Int(nullable: false),
                    SequenceOperationID = c.Int(nullable: false),
                    UserName = c.String(maxLength: 100),
                    FullName = c.String(maxLength: 100),
                    PositionName = c.String(maxLength: 255),
                    Roles = c.Int(nullable: false),
                    ElectricRank = c.Int(),
                    ElectricRankCard = c.String(maxLength: 255),
                    Signature = c.String(),
                    IsExternal = c.Boolean(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.Signature",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    SignatureContent = c.String(),
                    Type = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.TimeLineCycle",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Hour = c.Int(nullable: false),
                    CycleID = c.Int(nullable: false),
                    Active = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.CycleManagement", t => t.CycleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CycleID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.TrackingSyncMobileData",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    DeviceID = c.String(nullable: false),
                    SyncDate = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.UserPermission",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    PermissionKey = c.String(nullable: false, maxLength: 50),
                    PermissionName = c.String(nullable: false, maxLength: 255),
                    Active = c.Int(nullable: false),
                    ModuleValue = c.String(nullable: false, maxLength: 255),
                    SystemType = c.String(nullable: false, maxLength: 50),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkFinish",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkTicketID = c.Int(nullable: false),
                    ReturnWorkplaceTime = c.DateTime(nullable: false),
                    CheckReceivedTime = c.DateTime(nullable: false),
                    CheckCompletedTime = c.DateTime(nullable: false),
                    LeaderSignature = c.Int(),
                    CommanderSignature = c.Int(),
                    AllowedSignature = c.Int(),
                    ApproverSignature = c.Int(),
                    RepresentativeUnit = c.String(maxLength: 255),
                    RepresentativeUnitPositionName = c.String(maxLength: 255),
                    OperatorConfirmed1 = c.Boolean(),
                    OperatorConfirmed2 = c.Boolean(),
                    OperatorConfirmed3 = c.Boolean(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkTicket", t => t.WorkTicketID, cascadeDelete: true)
                .Index(t => t.WorkTicketID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

            CreateTable(
                "dbo.WorkFinishPicture",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WorkFinishID = c.Int(),
                    Attachments = c.String(maxLength: 500),
                    Position = c.Int(),
                    IsChecked = c.Boolean(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUserID = c.Int(),
                    InsertedAt = c.DateTime(nullable: false),
                    LastUpdatedUserID = c.Int(),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedUserID)
                .ForeignKey("dbo.Users", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.WorkFinish", t => t.WorkFinishID)
                .Index(t => t.WorkFinishID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.WorkFinish", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.WorkFinishPicture", "WorkFinishID", "dbo.WorkFinish");
            DropForeignKey("dbo.WorkFinishPicture", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkFinishPicture", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkFinish", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkFinish", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.UserPermission", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.UserPermission", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.TrackingSyncMobileData", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.TrackingSyncMobileData", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.TimeLineCycle", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.TimeLineCycle", "CycleID", "dbo.CycleManagement");
            DropForeignKey("dbo.TimeLineCycle", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Signature", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Signature", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.SequenceOperation", "WorkerID", "dbo.SequenceOperationParticipant");
            DropForeignKey("dbo.SequenceOperation", "LeaderID", "dbo.SequenceOperationParticipant");
            DropForeignKey("dbo.SequenceOperationParticipant", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.SequenceOperationParticipant", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.SequenceOperation", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.SequenceOperation", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.ReceiveWorkplace", "WorkUnitID", "dbo.Organization");
            DropForeignKey("dbo.ReceiveWorkplace", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.SafetyMeasure", "ReceiveWorkplaceID", "dbo.ReceiveWorkplace");
            DropForeignKey("dbo.SafetyMeasure", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.SafetyMeasure", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.ReceiveWorkplace", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.ReceiveWorkplace", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.PowerUnit", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.PowerUnit", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.PowerUnit", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Participant", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Participant", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OtherRequest", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OtherRequest", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationParameters", "ParameterGroupID", "dbo.ParameterGroup");
            DropForeignKey("dbo.OperationParameters", "OperationTimeLineID", "dbo.OperationTimeLine");
            DropForeignKey("dbo.OperationTimeLine", "OperationPositionID", "dbo.OperationPosition");
            DropForeignKey("dbo.OperationTimeLine", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationTimeLine", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.OperationTimeLine", "CycleID", "dbo.CycleManagement");
            DropForeignKey("dbo.OperationTimeLine", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationParameters", "OperationPositionID", "dbo.OperationPosition");
            DropForeignKey("dbo.OperationParameters", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationParameters", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.OperationParameters", "CycleID", "dbo.CycleManagement");
            DropForeignKey("dbo.OperationParameters", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Modules", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Modules", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.ManipulationTicket", "TicketStatusValue", "dbo.TicketManipulationStatus");
            DropForeignKey("dbo.ManipulationTicket", "RegistrationManipulationTicketID", "dbo.RegistrationManipulationTicket");
            DropForeignKey("dbo.ManipulationTicket", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.ManipulationTicket", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.ManipulationTicket", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.ManipulationTicket", "ApproveUnitID", "dbo.Organization");
            DropForeignKey("dbo.ManipulationTemplateTickets", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.ManipulationTemplateTickets", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.GuestUsers", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.GuestUsers", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.ExclusiveData", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.ExclusiveData", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.DeliveryReceiveShift", "SupervisorID", "dbo.Users");
            DropForeignKey("dbo.OperationSituation", "OperationDiaryID", "dbo.OperationDiary");
            DropForeignKey("dbo.OperationSituation", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationSituation", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationSituation", "AssignedToID", "dbo.Users");
            DropForeignKey("dbo.OperationSituation", "AssignedFromID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "OperationShiftID", "dbo.OperationShift");
            DropForeignKey("dbo.UsedTimes", "OperationShiftID", "dbo.OperationShift");
            DropForeignKey("dbo.UsedTimes", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.UsedTimes", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationShift", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationShift", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.OperationShift", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "OperationPositionID", "dbo.OperationPosition");
            DropForeignKey("dbo.OperationDiaryPositionShift", "PositionInShiftID", "dbo.PositionInShift");
            DropForeignKey("dbo.OperationDiaryPositionShift", "OperationDiaryID", "dbo.OperationDiary");
            DropForeignKey("dbo.OperationDiaryPositionShift", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiaryPositionShift", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiaryPositionShift", "AssignedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "OperationCrewID", "dbo.OperationCrew");
            DropForeignKey("dbo.OperationCrew", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationCrew", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.OperationCrew", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.ExistInShift", "RegistrationTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.ExistInShift", "OperationDiaryID", "dbo.OperationDiary");
            DropForeignKey("dbo.ExistInShift", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.IssueOfShiftDetail", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.IssueOfShiftDetail", "ExistInShiftID", "dbo.ExistInShift");
            DropForeignKey("dbo.IssueOfShiftDetail", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.ExistInShift", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "DeliveryShiftMethodID", "dbo.DeliveryShiftMethod");
            DropForeignKey("dbo.DeliveryShiftMethod", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.DeliveryShiftMethod", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.DeliveryReceiveShift", "OperationDiaryID", "dbo.OperationDiary");
            DropForeignKey("dbo.OperationDiary", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationDiary", "AbnormalIncidentID", "dbo.AbnormalIncident");
            DropForeignKey("dbo.DeliveryReceiveShift", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.DeliveryReceiveShift", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.DeliveryAccepDevice", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.DeliveryAccepDevice", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.CancelTicketInfo", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.CancelTicketInfo", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.AllowedInfo", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.WorkTicket", "WorkTicketFeatureID", "dbo.WorkTicketFeatures");
            DropForeignKey("dbo.WorkTicketFeatures", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkTicketFeatures", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkPlacement", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.WorkPlacement", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkPlacement", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkerInformation", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.WorkerInformation", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkerInformation", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkDaily", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.WorkDaily", "WorkLocationID", "dbo.ContentPosition");
            DropForeignKey("dbo.StaffInformation", "WorkDailyID", "dbo.WorkDaily");
            DropForeignKey("dbo.StaffInformation", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.StaffInformation", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkDaily", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkDaily", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WarnAndSafe", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.WarnAndSafe", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WarnAndSafe", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkTicket", "WorkTicketStatus", "dbo.TicketStatus");
            DropForeignKey("dbo.WorkTicket", "RegistrationTicketID", "dbo.RegistrationTicket");
            DropForeignKey("dbo.TicketStatusHistory", "StatusValue", "dbo.TicketStatus");
            DropForeignKey("dbo.TicketStatusHistory", "RegistrationTicketID", "dbo.RegistrationTicket");
            DropForeignKey("dbo.TicketStatusHistory", "RegistrationManipulationID", "dbo.RegistrationManipulationTicket");
            DropForeignKey("dbo.RegistrationManipulationTicket", "TicketStatusValue", "dbo.TicketManipulationStatus");
            DropForeignKey("dbo.TicketManipulationStatus", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.TicketManipulationStatus", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationManipulationTicket", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationManipulationTicket", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.RegistrationManipulationTicket", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationManipulationTicket", "ApproveUnitID", "dbo.Organization");
            DropForeignKey("dbo.RegistrationManipulationTicket", "ApprovedUserID", "dbo.Users");
            DropForeignKey("dbo.TicketStatusHistory", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.TicketStatusHistory", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationTicket", "StatusValue", "dbo.TicketStatus");
            DropForeignKey("dbo.TicketStatus", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.TicketStatus", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationTicket", "RegistrationUnitID", "dbo.Organization");
            DropForeignKey("dbo.RegistrationTicket", "ReceiveUnitID", "dbo.Organization");
            DropForeignKey("dbo.OtherRequirements", "RegistrationTicketID", "dbo.RegistrationTicket");
            DropForeignKey("dbo.OtherRequirements", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OtherRequirements", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationTicket", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.IdentificationAndSafetyMeasures", "RegistrationTicketID", "dbo.RegistrationTicket");
            DropForeignKey("dbo.IdentificationAndSafetyMeasures", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.IdentificationAndSafetyMeasures", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RegistrationTicket", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.RegistrationTicket", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Stakeholders", "RegistrationTicketID", "dbo.RegistrationTicket");
            DropForeignKey("dbo.Stakeholders", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Stakeholders", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Stakeholders", "ContentPositionID", "dbo.ContentPosition");
            DropForeignKey("dbo.Stakeholders", "AssignToUserID", "dbo.Users");
            DropForeignKey("dbo.ContentPosition", "RegistrationTicketID", "dbo.RegistrationTicket");
            DropForeignKey("dbo.ContentPosition", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.ContentPosition", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.LeadershipUser", "WorkTicketID", "dbo.WorkTicket");
            DropForeignKey("dbo.LeadershipUser", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.LeadershipUser", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkTicket", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkTicket", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.WorkTicket", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.WorkTicket", "ApprovedUnitID", "dbo.Organization");
            DropForeignKey("dbo.AllowedInfo", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.AllowedInfo", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.AllowedInfoDetail", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.AllowedInfoDetail", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.AllowedInfoDetail", "AllowedInforID", "dbo.AllowedInfo");
            DropForeignKey("dbo.Parameters", "ParameterGroupID", "dbo.ParameterGroup");
            DropForeignKey("dbo.ParameterGroup", "OperationPositionID", "dbo.OperationPosition");
            DropForeignKey("dbo.ParameterGroup", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.ParameterGroup", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.ParameterGroup", "CycleID", "dbo.CycleManagement");
            DropForeignKey("dbo.ParameterGroup", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Parameters", "OperationPositionID", "dbo.OperationPosition");
            DropForeignKey("dbo.PositionInShift", "OperationPositionID", "dbo.OperationPosition");
            DropForeignKey("dbo.PositionInShift", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.PositionInShift", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationPosition", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.OperationPosition", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.OperationPosition", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Parameters", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Parameters", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.Parameters", "CycleID", "dbo.CycleManagement");
            DropForeignKey("dbo.CycleManagement", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.CycleFactory", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.CycleFactory", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.CycleFactory", "CycleID", "dbo.CycleManagement");
            DropForeignKey("dbo.CycleFactory", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.CycleManagement", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Parameters", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.AlertDetails", "ParameterID", "dbo.Parameters");
            DropForeignKey("dbo.AlertDetails", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.AlertDetails", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.AlarmSettings", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.AlarmSettings", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.AbnormalIncident", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.AbnormalIncident", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Users", "TeamID", "dbo.Organization");
            DropForeignKey("dbo.RoleInUsers", "UserRoleID", "dbo.UserRoles");
            DropForeignKey("dbo.UserRoles", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.RoleInUsers", "UserID", "dbo.Users");
            DropForeignKey("dbo.Users", "ProductionPlantID", "dbo.Organization");
            DropForeignKey("dbo.Users", "GroupID", "dbo.Organization");
            DropForeignKey("dbo.Users", "FactoryID", "dbo.Factory");
            DropForeignKey("dbo.Factory", "ProductionPlantID", "dbo.Organization");
            DropForeignKey("dbo.Factory", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Factory", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Factory", "CompanyID", "dbo.Organization");
            DropForeignKey("dbo.Users", "CompanyID", "dbo.Organization");
            DropForeignKey("dbo.Organization", "ParentID", "dbo.Organization");
            DropForeignKey("dbo.Organization", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Organization", "CreatedUserID", "dbo.Users");
            DropForeignKey("dbo.Organization", "AttachmentID", "dbo.Attachment");
            DropForeignKey("dbo.Attachment", "LastUpdatedUserID", "dbo.Users");
            DropForeignKey("dbo.Attachment", "CreatedUserID", "dbo.Users");
            DropIndex("dbo.WorkFinishPicture", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkFinishPicture", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkFinishPicture", new[] { "WorkFinishID" });
            DropIndex("dbo.WorkFinish", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkFinish", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkFinish", new[] { "WorkTicketID" });
            DropIndex("dbo.UserPermission", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.UserPermission", new[] { "CreatedUserID" });
            DropIndex("dbo.TrackingSyncMobileData", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.TrackingSyncMobileData", new[] { "CreatedUserID" });
            DropIndex("dbo.TimeLineCycle", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.TimeLineCycle", new[] { "CreatedUserID" });
            DropIndex("dbo.TimeLineCycle", new[] { "CycleID" });
            DropIndex("dbo.Signature", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Signature", new[] { "CreatedUserID" });
            DropIndex("dbo.SequenceOperationParticipant", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.SequenceOperationParticipant", new[] { "CreatedUserID" });
            DropIndex("dbo.SequenceOperation", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.SequenceOperation", new[] { "CreatedUserID" });
            DropIndex("dbo.SequenceOperation", new[] { "WorkerID" });
            DropIndex("dbo.SequenceOperation", new[] { "LeaderID" });
            DropIndex("dbo.SafetyMeasure", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.SafetyMeasure", new[] { "CreatedUserID" });
            DropIndex("dbo.SafetyMeasure", new[] { "ReceiveWorkplaceID" });
            DropIndex("dbo.ReceiveWorkplace", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ReceiveWorkplace", new[] { "CreatedUserID" });
            DropIndex("dbo.ReceiveWorkplace", new[] { "WorkUnitID" });
            DropIndex("dbo.ReceiveWorkplace", new[] { "WorkTicketID" });
            DropIndex("dbo.PowerUnit", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.PowerUnit", new[] { "CreatedUserID" });
            DropIndex("dbo.PowerUnit", new[] { "FactoryID" });
            DropIndex("dbo.Participant", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Participant", new[] { "CreatedUserID" });
            DropIndex("dbo.OtherRequest", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OtherRequest", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationTimeLine", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationTimeLine", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationTimeLine", new[] { "CycleID" });
            DropIndex("dbo.OperationTimeLine", new[] { "OperationPositionID" });
            DropIndex("dbo.OperationTimeLine", new[] { "FactoryID" });
            DropIndex("dbo.OperationParameters", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationParameters", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationParameters", new[] { "CycleID" });
            DropIndex("dbo.OperationParameters", new[] { "OperationPositionID" });
            DropIndex("dbo.OperationParameters", new[] { "FactoryID" });
            DropIndex("dbo.OperationParameters", new[] { "OperationTimeLineID" });
            DropIndex("dbo.OperationParameters", new[] { "ParameterGroupID" });
            DropIndex("dbo.Modules", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Modules", new[] { "CreatedUserID" });
            DropIndex("dbo.ManipulationTicket", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ManipulationTicket", new[] { "CreatedUserID" });
            DropIndex("dbo.ManipulationTicket", new[] { "TicketStatusValue" });
            DropIndex("dbo.ManipulationTicket", new[] { "ApproveUnitID" });
            DropIndex("dbo.ManipulationTicket", new[] { "FactoryID" });
            DropIndex("dbo.ManipulationTicket", new[] { "RegistrationManipulationTicketID" });
            DropIndex("dbo.ManipulationTemplateTickets", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ManipulationTemplateTickets", new[] { "CreatedUserID" });
            DropIndex("dbo.GuestUsers", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.GuestUsers", new[] { "CreatedUserID" });
            DropIndex("dbo.ExclusiveData", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ExclusiveData", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationSituation", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationSituation", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationSituation", new[] { "AssignedToID" });
            DropIndex("dbo.OperationSituation", new[] { "AssignedFromID" });
            DropIndex("dbo.OperationSituation", new[] { "OperationDiaryID" });
            DropIndex("dbo.UsedTimes", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.UsedTimes", new[] { "CreatedUserID" });
            DropIndex("dbo.UsedTimes", new[] { "OperationShiftID" });
            DropIndex("dbo.OperationShift", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationShift", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationShift", new[] { "FactoryID" });
            DropIndex("dbo.OperationDiaryPositionShift", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationDiaryPositionShift", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationDiaryPositionShift", new[] { "AssignedUserID" });
            DropIndex("dbo.OperationDiaryPositionShift", new[] { "PositionInShiftID" });
            DropIndex("dbo.OperationDiaryPositionShift", new[] { "OperationDiaryID" });
            DropIndex("dbo.OperationCrew", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationCrew", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationCrew", new[] { "FactoryID" });
            DropIndex("dbo.IssueOfShiftDetail", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.IssueOfShiftDetail", new[] { "CreatedUserID" });
            DropIndex("dbo.IssueOfShiftDetail", new[] { "ExistInShiftID" });
            DropIndex("dbo.ExistInShift", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ExistInShift", new[] { "CreatedUserID" });
            DropIndex("dbo.ExistInShift", new[] { "RegistrationTicketID" });
            DropIndex("dbo.ExistInShift", new[] { "OperationDiaryID" });
            DropIndex("dbo.DeliveryShiftMethod", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.DeliveryShiftMethod", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationDiary", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationDiary", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationDiary", new[] { "DeliveryShiftMethodID" });
            DropIndex("dbo.OperationDiary", new[] { "AbnormalIncidentID" });
            DropIndex("dbo.OperationDiary", new[] { "OperationCrewID" });
            DropIndex("dbo.OperationDiary", new[] { "OperationShiftID" });
            DropIndex("dbo.OperationDiary", new[] { "OperationPositionID" });
            DropIndex("dbo.OperationDiary", new[] { "FactoryID" });
            DropIndex("dbo.DeliveryReceiveShift", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.DeliveryReceiveShift", new[] { "CreatedUserID" });
            DropIndex("dbo.DeliveryReceiveShift", new[] { "SupervisorID" });
            DropIndex("dbo.DeliveryReceiveShift", new[] { "OperationDiaryID" });
            DropIndex("dbo.DeliveryAccepDevice", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.DeliveryAccepDevice", new[] { "CreatedUserID" });
            DropIndex("dbo.CancelTicketInfo", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.CancelTicketInfo", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkTicketFeatures", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkTicketFeatures", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkPlacement", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkPlacement", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkPlacement", new[] { "WorkTicketID" });
            DropIndex("dbo.WorkerInformation", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkerInformation", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkerInformation", new[] { "WorkTicketID" });
            DropIndex("dbo.StaffInformation", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.StaffInformation", new[] { "CreatedUserID" });
            DropIndex("dbo.StaffInformation", new[] { "WorkDailyID" });
            DropIndex("dbo.WorkDaily", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkDaily", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkDaily", new[] { "WorkLocationID" });
            DropIndex("dbo.WorkDaily", new[] { "WorkTicketID" });
            DropIndex("dbo.WarnAndSafe", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WarnAndSafe", new[] { "CreatedUserID" });
            DropIndex("dbo.WarnAndSafe", new[] { "WorkTicketID" });
            DropIndex("dbo.TicketManipulationStatus", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.TicketManipulationStatus", new[] { "CreatedUserID" });
            DropIndex("dbo.RegistrationManipulationTicket", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.RegistrationManipulationTicket", new[] { "CreatedUserID" });
            DropIndex("dbo.RegistrationManipulationTicket", new[] { "TicketStatusValue" });
            DropIndex("dbo.RegistrationManipulationTicket", new[] { "ApprovedUserID" });
            DropIndex("dbo.RegistrationManipulationTicket", new[] { "ApproveUnitID" });
            DropIndex("dbo.RegistrationManipulationTicket", new[] { "FactoryID" });
            DropIndex("dbo.TicketStatusHistory", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.TicketStatusHistory", new[] { "CreatedUserID" });
            DropIndex("dbo.TicketStatusHistory", new[] { "StatusValue" });
            DropIndex("dbo.TicketStatusHistory", new[] { "RegistrationManipulationID" });
            DropIndex("dbo.TicketStatusHistory", new[] { "RegistrationTicketID" });
            DropIndex("dbo.TicketStatus", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.TicketStatus", new[] { "CreatedUserID" });
            DropIndex("dbo.OtherRequirements", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OtherRequirements", new[] { "CreatedUserID" });
            DropIndex("dbo.OtherRequirements", new[] { "RegistrationTicketID" });
            DropIndex("dbo.IdentificationAndSafetyMeasures", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.IdentificationAndSafetyMeasures", new[] { "CreatedUserID" });
            DropIndex("dbo.IdentificationAndSafetyMeasures", new[] { "RegistrationTicketID" });
            DropIndex("dbo.Stakeholders", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Stakeholders", new[] { "CreatedUserID" });
            DropIndex("dbo.Stakeholders", new[] { "ContentPositionID" });
            DropIndex("dbo.Stakeholders", new[] { "RegistrationTicketID" });
            DropIndex("dbo.Stakeholders", new[] { "AssignToUserID" });
            DropIndex("dbo.ContentPosition", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ContentPosition", new[] { "CreatedUserID" });
            DropIndex("dbo.ContentPosition", new[] { "RegistrationTicketID" });
            DropIndex("dbo.RegistrationTicket", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.RegistrationTicket", new[] { "CreatedUserID" });
            DropIndex("dbo.RegistrationTicket", new[] { "StatusValue" });
            DropIndex("dbo.RegistrationTicket", new[] { "RegistrationUnitID" });
            DropIndex("dbo.RegistrationTicket", new[] { "ReceiveUnitID" });
            DropIndex("dbo.RegistrationTicket", new[] { "FactoryID" });
            DropIndex("dbo.LeadershipUser", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.LeadershipUser", new[] { "CreatedUserID" });
            DropIndex("dbo.LeadershipUser", new[] { "WorkTicketID" });
            DropIndex("dbo.WorkTicket", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.WorkTicket", new[] { "CreatedUserID" });
            DropIndex("dbo.WorkTicket", new[] { "WorkTicketFeatureID" });
            DropIndex("dbo.WorkTicket", new[] { "WorkTicketStatus" });
            DropIndex("dbo.WorkTicket", new[] { "ApprovedUnitID" });
            DropIndex("dbo.WorkTicket", new[] { "FactoryID" });
            DropIndex("dbo.WorkTicket", new[] { "RegistrationTicketID" });
            DropIndex("dbo.AllowedInfoDetail", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.AllowedInfoDetail", new[] { "CreatedUserID" });
            DropIndex("dbo.AllowedInfoDetail", new[] { "AllowedInforID" });
            DropIndex("dbo.AllowedInfo", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.AllowedInfo", new[] { "CreatedUserID" });
            DropIndex("dbo.AllowedInfo", new[] { "WorkTicketID" });
            DropIndex("dbo.ParameterGroup", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ParameterGroup", new[] { "CreatedUserID" });
            DropIndex("dbo.ParameterGroup", new[] { "CycleID" });
            DropIndex("dbo.ParameterGroup", new[] { "OperationPositionID" });
            DropIndex("dbo.ParameterGroup", new[] { "FactoryID" });
            DropIndex("dbo.PositionInShift", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.PositionInShift", new[] { "CreatedUserID" });
            DropIndex("dbo.PositionInShift", new[] { "OperationPositionID" });
            DropIndex("dbo.OperationPosition", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.OperationPosition", new[] { "CreatedUserID" });
            DropIndex("dbo.OperationPosition", new[] { "FactoryID" });
            DropIndex("dbo.CycleFactory", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.CycleFactory", new[] { "CreatedUserID" });
            DropIndex("dbo.CycleFactory", new[] { "FactoryID" });
            DropIndex("dbo.CycleFactory", new[] { "CycleID" });
            DropIndex("dbo.CycleManagement", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.CycleManagement", new[] { "CreatedUserID" });
            DropIndex("dbo.Parameters", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Parameters", new[] { "CreatedUserID" });
            DropIndex("dbo.Parameters", new[] { "ParameterGroupID" });
            DropIndex("dbo.Parameters", new[] { "CycleID" });
            DropIndex("dbo.Parameters", new[] { "OperationPositionID" });
            DropIndex("dbo.Parameters", new[] { "FactoryID" });
            DropIndex("dbo.AlertDetails", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.AlertDetails", new[] { "CreatedUserID" });
            DropIndex("dbo.AlertDetails", new[] { "ParameterID" });
            DropIndex("dbo.AlarmSettings", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.AlarmSettings", new[] { "CreatedUserID" });
            DropIndex("dbo.UserRoles", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.UserRoles", new[] { "CreatedUserID" });
            DropIndex("dbo.RoleInUsers", new[] { "UserID" });
            DropIndex("dbo.RoleInUsers", new[] { "UserRoleID" });
            DropIndex("dbo.Factory", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Factory", new[] { "CreatedUserID" });
            DropIndex("dbo.Factory", new[] { "ProductionPlantID" });
            DropIndex("dbo.Factory", new[] { "CompanyID" });
            DropIndex("dbo.Attachment", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Attachment", new[] { "CreatedUserID" });
            DropIndex("dbo.Organization", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Organization", new[] { "CreatedUserID" });
            DropIndex("dbo.Organization", new[] { "AttachmentID" });
            DropIndex("dbo.Organization", new[] { "ParentID" });
            DropIndex("dbo.Users", new[] { "FactoryID" });
            DropIndex("dbo.Users", new[] { "GroupID" });
            DropIndex("dbo.Users", new[] { "TeamID" });
            DropIndex("dbo.Users", new[] { "ProductionPlantID" });
            DropIndex("dbo.Users", new[] { "CompanyID" });
            DropIndex("dbo.AbnormalIncident", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.AbnormalIncident", new[] { "CreatedUserID" });
            DropTable("dbo.WorkFinishPicture");
            DropTable("dbo.WorkFinish");
            DropTable("dbo.UserPermission");
            DropTable("dbo.TrackingSyncMobileData");
            DropTable("dbo.TimeLineCycle");
            DropTable("dbo.Signature");
            DropTable("dbo.SequenceOperationParticipant");
            DropTable("dbo.SequenceOperation");
            DropTable("dbo.SafetyMeasure");
            DropTable("dbo.ReceiveWorkplace");
            DropTable("dbo.PowerUnit");
            DropTable("dbo.Participant");
            DropTable("dbo.OtherRequest");
            DropTable("dbo.OperationTimeLine");
            DropTable("dbo.OperationParameters");
            DropTable("dbo.Modules");
            DropTable("dbo.ManipulationTicket");
            DropTable("dbo.ManipulationTemplateTickets");
            DropTable("dbo.GuestUsers");
            DropTable("dbo.ExclusiveData");
            DropTable("dbo.OperationSituation");
            DropTable("dbo.UsedTimes");
            DropTable("dbo.OperationShift");
            DropTable("dbo.OperationDiaryPositionShift");
            DropTable("dbo.OperationCrew");
            DropTable("dbo.IssueOfShiftDetail");
            DropTable("dbo.ExistInShift");
            DropTable("dbo.DeliveryShiftMethod");
            DropTable("dbo.OperationDiary");
            DropTable("dbo.DeliveryReceiveShift");
            DropTable("dbo.DeliveryAccepDevice");
            DropTable("dbo.CancelTicketInfo");
            DropTable("dbo.WorkTicketFeatures");
            DropTable("dbo.WorkPlacement");
            DropTable("dbo.WorkerInformation");
            DropTable("dbo.StaffInformation");
            DropTable("dbo.WorkDaily");
            DropTable("dbo.WarnAndSafe");
            DropTable("dbo.TicketManipulationStatus");
            DropTable("dbo.RegistrationManipulationTicket");
            DropTable("dbo.TicketStatusHistory");
            DropTable("dbo.TicketStatus");
            DropTable("dbo.OtherRequirements");
            DropTable("dbo.IdentificationAndSafetyMeasures");
            DropTable("dbo.Stakeholders");
            DropTable("dbo.ContentPosition");
            DropTable("dbo.RegistrationTicket");
            DropTable("dbo.LeadershipUser");
            DropTable("dbo.WorkTicket");
            DropTable("dbo.AllowedInfoDetail");
            DropTable("dbo.AllowedInfo");
            DropTable("dbo.ParameterGroup");
            DropTable("dbo.PositionInShift");
            DropTable("dbo.OperationPosition");
            DropTable("dbo.CycleFactory");
            DropTable("dbo.CycleManagement");
            DropTable("dbo.Parameters");
            DropTable("dbo.AlertDetails");
            DropTable("dbo.AlarmSettings");
            DropTable("dbo.UserRoles");
            DropTable("dbo.RoleInUsers");
            DropTable("dbo.Factory");
            DropTable("dbo.Attachment");
            DropTable("dbo.Organization");
            DropTable("dbo.Users");
            DropTable("dbo.AbnormalIncident");
        }
    }
}
