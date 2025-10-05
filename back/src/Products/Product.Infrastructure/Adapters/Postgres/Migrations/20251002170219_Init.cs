using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Products.Infrastructure.Adapters.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "measure_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measure_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    measure_type_id = table.Column<int>(type: "integer", nullable: false),
                    is_archive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_measure_types_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "measure_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "measure_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "weight" },
                    { 2, "liquid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsArchive",
                table: "products",
                column: "is_archive");

            migrationBuilder.CreateIndex(
                name: "IX_products_measure_type_id",
                table: "products",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_name",
                table: "products",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "measure_types");
        }
    }
}
