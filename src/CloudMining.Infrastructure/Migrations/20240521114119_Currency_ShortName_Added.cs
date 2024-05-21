using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudMining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Currency_ShortName_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Currencies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("62c520f6-499a-499e-86c2-b2af9c8eba42"),
                column: "ShortName",
                value: "LTC");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("8927702d-ae2e-4a5a-af19-2e6fa1824648"),
                column: "ShortName",
                value: "ETH");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("9a3e4016-240e-44e3-a002-0ee2cff499a3"),
                column: "ShortName",
                value: "BTC");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("a5450179-3bff-4645-9209-04acc6168c5b"),
                column: "ShortName",
                value: "DOGE");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("e23a58c6-9cef-4c6f-94fc-8577a4b4fb84"),
                column: "ShortName",
                value: "₽");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("f1debadf-a2c3-4908-a11c-8329df252fb8"),
                column: "ShortName",
                value: "$");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Currencies");
        }
    }
}
