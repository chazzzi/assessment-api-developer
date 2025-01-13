namespace assessment_platform_developer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Address = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        City = c.String(),
                        State = c.String(nullable: false),
                        Zip = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        Notes = c.String(),
                        ContactName = c.String(),
                        ContactPhone = c.String(),
                        ContactEmail = c.String(),
                        ContactTitle = c.String(),
                        ContactNotes = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Customers");
        }
    }
}
