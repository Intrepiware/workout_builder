using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "workouts");

            migrationBuilder.RenameTable(
                name: "Timing",
                newName: "Timing",
                newSchema: "workouts");

            migrationBuilder.RenameTable(
                name: "Focus",
                newName: "Focus",
                newSchema: "workouts");

            migrationBuilder.RenameTable(
                name: "Exercise",
                newName: "Exercise",
                newSchema: "workouts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Timing",
                schema: "workouts",
                newName: "Timing");

            migrationBuilder.RenameTable(
                name: "Focus",
                schema: "workouts",
                newName: "Focus");

            migrationBuilder.RenameTable(
                name: "Exercise",
                schema: "workouts",
                newName: "Exercise");
        }
    }
}
