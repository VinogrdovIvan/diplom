using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class jrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "cars",
                columns: new[] { "car_id", "brand", "color", "daily_rate", "is_available", "license_plate", "model", "year" },
                values: new object[,]
                {
                    { 1, "Toyota", "Black", 25.50m, true, "ABC123", "Camry", 2020 },
                    { 2, "Honda", "White", 23.75m, true, "XYZ789", "Accord", 2019 },
                    { 3, "Ford", "Blue", 20.00m, true, "DEF456", "Focus", 2021 }
                });

            migrationBuilder.InsertData(
                table: "drivers",
                columns: new[] { "driver_id", "first_name", "hire_date", "is_available", "last_name", "license_number", "phone" },
                values: new object[,]
                {
                    { 1, "Иван", new DateOnly(2020, 1, 15), true, "Иванов", "DL12345", "+79001234567" },
                    { 2, "Петр", new DateOnly(2019, 5, 20), true, "Петров", "DL67890", "+79007654321" },
                    { 3, "Сергей", new DateOnly(2021, 3, 10), true, "Сергеев", "DL54321", "+79005556677" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cars",
                keyColumn: "car_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cars",
                keyColumn: "car_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cars",
                keyColumn: "car_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "drivers",
                keyColumn: "driver_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "drivers",
                keyColumn: "driver_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "drivers",
                keyColumn: "driver_id",
                keyValue: 3);
        }
    }
}
