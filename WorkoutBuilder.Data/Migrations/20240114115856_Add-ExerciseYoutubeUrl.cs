using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseYoutubeUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                schema: "workouts",
                table: "Exercise",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                schema: "workouts",
                table: "Exercise");
        }
    }
}
