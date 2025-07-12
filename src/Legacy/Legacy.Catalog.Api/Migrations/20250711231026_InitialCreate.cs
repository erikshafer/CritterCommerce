using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legacy.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "legacy_catalog");

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "legacy_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "legacy_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventories",
                schema: "legacy_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                schema: "legacy_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "media",
                schema: "legacy_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_url_1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prices",
                schema: "legacy_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_prices", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brands",
                schema: "legacy_catalog");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "legacy_catalog");

            migrationBuilder.DropTable(
                name: "inventories",
                schema: "legacy_catalog");

            migrationBuilder.DropTable(
                name: "items",
                schema: "legacy_catalog");

            migrationBuilder.DropTable(
                name: "media",
                schema: "legacy_catalog");

            migrationBuilder.DropTable(
                name: "prices",
                schema: "legacy_catalog");
        }
    }
}
