using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudMining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MarketData_Date_IsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketData_Date",
                table: "MarketData");

            migrationBuilder.CreateIndex(
                name: "IX_MarketData_From_To_Date",
                table: "MarketData",
                columns: new[] { "From", "To", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketData_From_To_Date",
                table: "MarketData");

            migrationBuilder.CreateIndex(
                name: "IX_MarketData_Date",
                table: "MarketData",
                column: "Date",
                unique: true);
        }
    }
}
