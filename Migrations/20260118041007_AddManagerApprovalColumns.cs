using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp_Anti.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerApprovalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "dbo",
                table: "DiscardApprovals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MgrWarehouse",
                schema: "dbo",
                table: "DiscardApprovals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MgrDfo",
                schema: "dbo",
                table: "DiscardApprovals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MgrVp",
                schema: "dbo",
                table: "DiscardApprovals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "DiscardApprovals");

            migrationBuilder.DropColumn(
                name: "MgrWarehouse",
                schema: "dbo",
                table: "DiscardApprovals");

            migrationBuilder.DropColumn(
                name: "MgrDfo",
                schema: "dbo",
                table: "DiscardApprovals");

            migrationBuilder.DropColumn(
                name: "MgrVp",
                schema: "dbo",
                table: "DiscardApprovals");
        }
    }
}
