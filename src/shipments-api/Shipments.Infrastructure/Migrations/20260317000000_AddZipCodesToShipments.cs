using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddZipCodesToShipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginZipCode",
                table: "shipments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationZipCode",
                table: "shipments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginZipCode",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "DestinationZipCode",
                table: "shipments");
        }
    }
}
