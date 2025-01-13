namespace assessment_platform_developer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCountryAndStateTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.States", "CountryID", "dbo.Countries");
            DropIndex("dbo.States", new[] { "CountryID" });
            DropTable("dbo.States");
            DropTable("dbo.Countries");
        }
    }
}
