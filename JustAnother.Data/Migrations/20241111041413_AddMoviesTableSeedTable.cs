using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JustAnother.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMoviesTableSeedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsOscarWinner = table.Column<bool>(type: "bit", nullable: false),
                    InitialRelease = table.Column<DateOnly>(type: "date", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "CategoryId", "Description", "InitialRelease", "IsOscarWinner", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.", new DateOnly(2020, 1, 1), true, "Action Movie 1" },
                    { 2, 2, "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.", new DateOnly(1986, 5, 5), false, "Adventure Movie 1" },
                    { 3, 3, "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.", new DateOnly(1960, 5, 5), false, "SciFi Movie 1" },
                    { 4, 2, "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.", new DateOnly(1986, 9, 5), true, "Adventure Movie 2" },
                    { 5, 1, "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.", new DateOnly(2003, 5, 5), false, "Action Movie 2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CategoryId",
                table: "Movies",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
