using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePartsList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
update workouts.ExercisePart
set PartId = 18
where partId = 1
	and ExerciseId not in (
		select ExerciseId
		from workouts.ExercisePart
		where PartId = 18
	)


update workouts.Exercise
set FocusPartId = 18
where FocusPartId = 1;

delete
from workouts.Part
where id = 1;

update workouts.ExercisePart
set PartId = 18
where PartId = 2
	and ExerciseId not in (
		select ExerciseId
		from workouts.ExercisePart
		where PartId = 18
	)

update workouts.Exercise
set FocusPartId = 18
where FocusPartId = 2;

delete
from workouts.Part
where id = 2;

update workouts.Part
set IsMuscle = 1
where Id = 18

update workouts.Part
set Name = 'Back'
where Id = 3
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
