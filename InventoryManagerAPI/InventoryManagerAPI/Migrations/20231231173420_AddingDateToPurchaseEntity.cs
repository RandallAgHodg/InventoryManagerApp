using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingDateToPurchaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 31, 17, 34, 18, 187, DateTimeKind.Utc).AddTicks(7543));

            migrationBuilder.AlterColumn<DateTime>(
                name: "inserted_at",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 31, 18, 34, 18, 179, DateTimeKind.Local).AddTicks(3009),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 11, 27, 11, 11, 28, 350, DateTimeKind.Local).AddTicks(8060));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "purchases");

            migrationBuilder.AlterColumn<DateTime>(
                name: "inserted_at",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 11, 27, 11, 11, 28, 350, DateTimeKind.Local).AddTicks(8060),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 31, 18, 34, 18, 179, DateTimeKind.Local).AddTicks(3009));
        }
    }
}
