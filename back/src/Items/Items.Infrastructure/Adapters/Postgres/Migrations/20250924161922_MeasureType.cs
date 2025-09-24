using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Items.Infrastructure.Adapters.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MeasureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "measure_type_id",
                table: "items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE items 
                SET measure_type_id = 
                    CASE 
                        WHEN measure_type = 'weight' THEN 1
                        WHEN measure_type = 'liquid' THEN 2
                        ELSE 1 -- значение по умолчанию, если есть неизвестные значения
                    END
            ");

            migrationBuilder.DropColumn(
                name: "measure_type",
                table: "items");

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

            migrationBuilder.InsertData(
                table: "measure_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "weight" },
                    { 2, "liquid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_items_measure_type_id",
                table: "items",
                column: "measure_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_items_measure_types_measure_type_id",
                table: "items",
                column: "measure_type_id",
                principalTable: "measure_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_measure_types_measure_type_id",
                table: "items");

            migrationBuilder.DropTable(
                name: "measure_types");

            migrationBuilder.DropIndex(
                name: "IX_items_measure_type_id",
                table: "items");

            migrationBuilder.AddColumn<string>(
                name: "measure_type",
                table: "items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE items 
                SET measure_type = 
                    CASE 
                        WHEN measure_type_id = 1 THEN 'weight'
                        WHEN measure_type_id = 2  THEN 'liquid'
                        ELSE 1 -- значение по умолчанию, если есть неизвестные значения
                    END
            ");

            migrationBuilder.DropColumn(
                name: "measure_type_id",
                table: "items");
        }
    }
}
