using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingSaleAndSaleDetailsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 14, 14, 34, 169, DateTimeKind.Utc).AddTicks(4646),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 31, 17, 34, 18, 187, DateTimeKind.Utc).AddTicks(7543));

            migrationBuilder.AlterColumn<DateTime>(
                name: "inserted_at",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 15, 14, 34, 149, DateTimeKind.Local).AddTicks(921),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 31, 18, 34, 18, 179, DateTimeKind.Local).AddTicks(3009));

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(252)", maxLength: 252, nullable: false),
                    deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 1, 1, 14, 14, 34, 170, DateTimeKind.Utc).AddTicks(5122)),
                    deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sales_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sales_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "sale_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sale_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    amount = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaleId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sale_details_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sale_details_sales_SaleId1",
                        column: x => x.SaleId1,
                        principalTable: "sales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sale_details_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_sale_details_ProductId",
                table: "sale_details",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_details_sale_id",
                table: "sale_details",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_details_SaleId1",
                table: "sale_details",
                column: "SaleId1");

            migrationBuilder.CreateIndex(
                name: "IX_sales_ClientId",
                table: "sales",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_user_id",
                table: "sales",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_details");

            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 31, 17, 34, 18, 187, DateTimeKind.Utc).AddTicks(7543),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 1, 1, 14, 14, 34, 169, DateTimeKind.Utc).AddTicks(4646));

            migrationBuilder.AlterColumn<DateTime>(
                name: "inserted_at",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 31, 18, 34, 18, 179, DateTimeKind.Local).AddTicks(3009),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 1, 1, 15, 14, 34, 149, DateTimeKind.Local).AddTicks(921));
        }
    }
}
