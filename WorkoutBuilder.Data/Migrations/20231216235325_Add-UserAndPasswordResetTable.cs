using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndPasswordResetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                schema: "workouts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LockDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordResetRequest",
                schema: "workouts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordResetRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPasswordResetRequest_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "workouts",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_EmailAddress",
                schema: "workouts",
                table: "User",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordResetRequest_PublicId",
                schema: "workouts",
                table: "UserPasswordResetRequest",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordResetRequest_UserId",
                schema: "workouts",
                table: "UserPasswordResetRequest",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasswordResetRequest",
                schema: "workouts");

            migrationBuilder.DropTable(
                name: "User",
                schema: "workouts");
        }
    }
}
