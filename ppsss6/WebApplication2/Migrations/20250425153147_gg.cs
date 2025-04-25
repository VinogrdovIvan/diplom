using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class gg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "orders_ibfk_1",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "orders_ibfk_2",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "car_id",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "orders_ibfk_1",
                table: "orders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "orders_ibfk_2",
                table: "orders",
                column: "car_id",
                principalTable: "cars",
                principalColumn: "car_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "orders_ibfk_1",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "orders_ibfk_2",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "car_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "orders_ibfk_1",
                table: "orders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "orders_ibfk_2",
                table: "orders",
                column: "car_id",
                principalTable: "cars",
                principalColumn: "car_id");
        }
    }
}
