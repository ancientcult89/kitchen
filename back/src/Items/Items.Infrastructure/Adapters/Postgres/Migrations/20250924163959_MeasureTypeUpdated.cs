using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Items.Infrastructure.Adapters.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MeasureTypeUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_measure_types_measure_type_id",
                table: "items");

            migrationBuilder.AddForeignKey(
                name: "FK_items_measure_types_measure_type_id",
                table: "items",
                column: "measure_type_id",
                principalTable: "measure_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_measure_types_measure_type_id",
                table: "items");

            migrationBuilder.AddForeignKey(
                name: "FK_items_measure_types_measure_type_id",
                table: "items",
                column: "measure_type_id",
                principalTable: "measure_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
