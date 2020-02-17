using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BandAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bands",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Founded = table.Column<DateTime>(nullable: false),
                    MainGenre = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 400, nullable: true),
                    BandId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey( // setting up the one to many relationship here
                        name: "FK_Albums_Bands_BandId",
                        column: x => x.BandId,
                        principalTable: "Bands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bands",
                columns: new[] { "Id", "Founded", "MainGenre", "Name" },
                values: new object[,]
                {
                    { new Guid("12a1c828-aaa3-4940-98ab-3625347fac3a"), new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Heavy Metal", "Band 2" },
                    { new Guid("ed69f31c-7475-45c0-a062-55f7c7245af5"), new DateTime(1980, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pop", "Band 3" },
                    { new Guid("50b56f93-20b9-4b49-96ce-5556d2ccee6d"), new DateTime(1980, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Style 3", "Band 4" },
                    { new Guid("6ff5c7b0-5465-4e21-a346-4564b34dda62"), new DateTime(1980, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Style 4", "Band 5" },
                    { new Guid("cf7e2071-cadc-4cfa-a197-d37a708be991"), new DateTime(1980, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Style 5", "Band 6" }
                });

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "BandId", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("008f39cc-ae29-44f2-8929-9b13fda96079"), new Guid("12a1c828-aaa3-4940-98ab-3625347fac3a"), "Classic Piano Performance", "Green Piano" },
                    { new Guid("9b4d14b1-40d4-4886-8b6b-58d8f3cf173f"), new Guid("ed69f31c-7475-45c0-a062-55f7c7245af5"), "Testing Description", "Albumn Name" },
                    { new Guid("bfcb9600-907f-4606-ae2e-ed11db784eb9"), new Guid("6ff5c7b0-5465-4e21-a346-4564b34dda62"), "Ophera Music", "Lion King" },
                    { new Guid("fad373fc-f59e-4666-a6d0-ce84abad4ce2"), new Guid("cf7e2071-cadc-4cfa-a197-d37a708be991"), "One of the best heavy metal albums ever", "Master of Puppets" },
                    { new Guid("3e6e88e7-18f2-4532-8320-b81a09aee05f"), new Guid("cf7e2071-cadc-4cfa-a197-d37a708be991"), "Legen Soul Music", "King in the Heaven" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_BandId",
                table: "Albums",
                column: "BandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Bands");
        }
    }
}
