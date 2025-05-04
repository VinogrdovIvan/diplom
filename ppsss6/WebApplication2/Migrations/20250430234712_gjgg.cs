using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class gjgg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "driver_id",
                table: "orders",
                newName: "driver_id1");

            migrationBuilder.CreateTable(
                name: "car_driver",
                columns: table => new
                {
                    car_id = table.Column<int>(type: "int", nullable: false),
                    driver_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.car_id, x.driver_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "car_driver_ibfk_1",
                        column: x => x.car_id,
                        principalTable: "cars",
                        principalColumn: "car_id");
                    table.ForeignKey(
                        name: "car_driver_ibfk_2",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "driver_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "driver_id",
                table: "car_driver",
                column: "driver_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "car_driver");

            migrationBuilder.RenameIndex(
                name: "driver_id1",
                table: "orders",
                newName: "driver_id");
        }
    }
}
