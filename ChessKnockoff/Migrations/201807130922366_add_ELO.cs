namespace ChessKnockoff.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ELO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ELO", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ELO");
        }
    }
}
