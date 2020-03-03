namespace ServiceDemo1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActionsLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionId = c.Int(nullable: false),
                        ActionName = c.String(maxLength: 100),
                        ActionDate = c.DateTime(nullable: false),
                        IsFirst = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        FK_StatusId = c.Int(nullable: false),
                        TotalHour = c.Time(precision: 7),
                        StartAt = c.Time(precision: 7),
                        EndAt = c.Time(precision: 7),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkDays");
            DropTable("dbo.ActionsLogs");
        }
    }
}
