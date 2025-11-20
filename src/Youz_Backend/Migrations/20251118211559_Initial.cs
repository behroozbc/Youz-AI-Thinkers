using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Pgvector;

#nullable disable

namespace Youz_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "Landmarks",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geometry", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Landmarks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DescriptionChunks",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    TextEmbedding = table.Column<Vector>(type: "vector(3072)", nullable: false),
                    LandmarkID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescriptionChunks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DescriptionChunks_Landmarks_LandmarkID",
                        column: x => x.LandmarkID,
                        principalTable: "Landmarks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageChunks",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: false),
                    ImageEmbedding = table.Column<Vector>(type: "vector(3072)", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: false),
                    LandmarkID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageChunks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ImageChunks_Landmarks_LandmarkID",
                        column: x => x.LandmarkID,
                        principalTable: "Landmarks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionChunks_LandmarkID",
                table: "DescriptionChunks",
                column: "LandmarkID");

            migrationBuilder.CreateIndex(
                name: "IX_ImageChunks_LandmarkID",
                table: "ImageChunks",
                column: "LandmarkID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DescriptionChunks");

            migrationBuilder.DropTable(
                name: "ImageChunks");

            migrationBuilder.DropTable(
                name: "Landmarks");
        }
    }
}
