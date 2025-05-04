using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NpuApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Creations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreationScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false),
                    Uniqueness = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreationScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreationScores_Creations_CreationId",
                        column: x => x.CreationId,
                        principalTable: "Creations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreationScores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Username" },
                values: new object[] { new Guid("9ecbfde3-df9e-4e60-a220-558609f1fe56"), new DateTime(2025, 5, 2, 14, 46, 51, 542, DateTimeKind.Utc), "admin@oneringtorulethemallinnarnia.com", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Creations_UserId",
                table: "Creations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreationScores_CreationId_UserId",
                table: "CreationScores",
                columns: new[] { "CreationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreationScores_UserId",
                table: "CreationScores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreationScores");

            migrationBuilder.DropTable(
                name: "Creations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
