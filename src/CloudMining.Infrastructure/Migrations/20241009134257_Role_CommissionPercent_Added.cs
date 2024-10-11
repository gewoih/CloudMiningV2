using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudMining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Role_CommissionPercent_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercent",
                table: "AspNetRoles",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionPercent",
                table: "AspNetRoles");
        }
    }
}
