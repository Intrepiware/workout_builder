using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FocusPartId",
                schema: "workouts",
                table: "Exercise",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Part",
                schema: "workouts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMuscle = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Part", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExercisePart",
                schema: "workouts",
                columns: table => new
                {
                    ExerciseId = table.Column<long>(type: "bigint", nullable: false),
                    PartId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercisePart", x => new { x.ExerciseId, x.PartId });
                    table.ForeignKey(
                        name: "FK_ExercisePart_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "workouts",
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercisePart_Part_PartId",
                        column: x => x.PartId,
                        principalSchema: "workouts",
                        principalTable: "Part",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_FocusPartId",
                schema: "workouts",
                table: "Exercise",
                column: "FocusPartId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercisePart_PartId",
                schema: "workouts",
                table: "ExercisePart",
                column: "PartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Part_FocusPartId",
                schema: "workouts",
                table: "Exercise",
                column: "FocusPartId",
                principalSchema: "workouts",
                principalTable: "Part",
                principalColumn: "Id");

            migrationBuilder.Sql(@"
set identity_insert workouts.Part on;
insert into workouts.[Part] (Id, Name, IsMuscle) values (1,'Traps',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (2,'Deltoids',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (3,'Lats',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (4,'Biceps',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (5,'Triceps',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (6,'Forearms',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (7,'Chest',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (8,'Abs',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (9,'Quads',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (10,'Hamstrings',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (11,'Calves',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (12,'Glutes',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (13,'Back',1);
insert into workouts.[Part] (Id, Name, IsMuscle) values (14,'Wrists',0);
insert into workouts.[Part] (Id, Name, IsMuscle) values (15,'Knees',0);
insert into workouts.[Part] (Id, Name, IsMuscle) values (16,'Elbows',0);
insert into workouts.[Part] (Id, Name, IsMuscle) values (17,'Ankles',0);
set identity_insert workouts.Part off");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Part_FocusPartId",
                schema: "workouts",
                table: "Exercise");

            migrationBuilder.DropTable(
                name: "ExercisePart",
                schema: "workouts");

            migrationBuilder.DropTable(
                name: "Part",
                schema: "workouts");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_FocusPartId",
                schema: "workouts",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "FocusPartId",
                schema: "workouts",
                table: "Exercise");
        }
    }
}
