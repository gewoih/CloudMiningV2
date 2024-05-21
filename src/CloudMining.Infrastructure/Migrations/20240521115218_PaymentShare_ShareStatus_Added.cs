using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudMining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentShare_ShareStatus_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "PaymentShares");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PaymentShares",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentShares");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "PaymentShares",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
