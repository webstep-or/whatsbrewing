namespace WhatsBrewing.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialdbcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartDate = c.String(),
                        StartTime = c.String(),
                        EndTime = c.String(),
                        RoomId = c.Guid(),
                        Icon = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Room", t => t.RoomId)
                .Index(t => t.RoomId);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Brewery",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Country = c.String(),
                        RoomId = c.Guid(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Room", t => t.RoomId)
                .Index(t => t.RoomId);
            
            CreateTable(
                "dbo.Beer",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Style = c.String(),
                        Alcohol = c.String(),
                        BreweryId = c.Guid(),
                        TapSessionId = c.Guid(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brewery", t => t.BreweryId)
                .ForeignKey("dbo.TapSession", t => t.TapSessionId)
                .Index(t => t.BreweryId)
                .Index(t => t.TapSessionId);
            
            CreateTable(
                "dbo.TapSession",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartDate = c.String(),
                        StartTime = c.String(),
                        EndTime = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activity", "RoomId", "dbo.Room");
            DropForeignKey("dbo.Brewery", "RoomId", "dbo.Room");
            DropForeignKey("dbo.Beer", "TapSessionId", "dbo.TapSession");
            DropForeignKey("dbo.Beer", "BreweryId", "dbo.Brewery");
            DropIndex("dbo.Beer", new[] { "TapSessionId" });
            DropIndex("dbo.Beer", new[] { "BreweryId" });
            DropIndex("dbo.Brewery", new[] { "RoomId" });
            DropIndex("dbo.Activity", new[] { "RoomId" });
            DropTable("dbo.TapSession");
            DropTable("dbo.Beer");
            DropTable("dbo.Brewery");
            DropTable("dbo.Room");
            DropTable("dbo.Activity");
        }
    }
}
