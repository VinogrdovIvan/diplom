using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class sss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "orders_ibfk_3",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "driver_id",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "orders_ibfk_3",
                table: "orders",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "driver_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "orders_ibfk_3",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "driver_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "orders_ibfk_3",
                table: "orders",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "driver_id");
        }
    }
}
