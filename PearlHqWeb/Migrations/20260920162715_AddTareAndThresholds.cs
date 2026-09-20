using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PearlHqWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddTareAndThresholds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TargetFullWeightGrams",
                table: "Dish",
                type: "REAL",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TareEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DishId = table.Column<int>(type: "INTEGER", nullable: false),
                    RawWeightGrams = table.Column<double>(type: "REAL", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TareEvent_Dish_DishId",
                        column: x => x.DishId,
                        principalTable: "Dish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TareEvent_DishId",
                table: "TareEvent",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareEvent");

            migrationBuilder.DropColumn(
                name: "TargetFullWeightGrams",
                table: "Dish");
        }
    }
}
