using System.Security;
using WorkoutBuilder.Data;
using FocusEnum = WorkoutBuilder.Services.Models.Focus;

namespace WorkoutBuilder.Services.Impl
{
    public class ExerciseService : IExerciseService
    {
        public IUserContext UserContext { init; protected get; } = null!;
        public IRepository<Exercise> ExerciseRepository { init; protected get; } = null!;
        public async Task<long> Add(Exercise exercise)
        {
            if (!UserContext.CanManageAllExercises())
                throw new SecurityException();

            var equipment = exercise.Equipment.ToLower();
            var existingEquipment = ExerciseRepository.GetAll().Where(x => x.Equipment.ToLower() == equipment).Any();
            if (!existingEquipment)
                throw new ArgumentException("You must use an existing piece of equipment");

            await ExerciseRepository.Add(exercise);
            return exercise.Id;
        }

        public async Task Delete(long id)
        {
            if (!UserContext.CanManageAllExercises())
                throw new SecurityException();

            var exercise = await ExerciseRepository.GetById(id);

            if (exercise != null)
                await ExerciseRepository.Delete(exercise);

        }

        public List<Exercise> Search(int take, int skip, string? name = null, string? focus = null, string? equipment = null)
        {
            if (!UserContext.CanReadAllExercises())
                throw new SecurityException();

            var canManageExercises = UserContext.CanManageAllExercises();
            var query = ExerciseRepository.GetAll();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));
            if (!string.IsNullOrEmpty(focus) && Enum.TryParse<FocusEnum>(focus, out var focusParam))
                query = query.Where(x => x.FocusId == (byte)focusParam);
            if (!string.IsNullOrWhiteSpace(equipment))
                query = query.Where(x => x.Equipment == equipment);

            return query.OrderBy(x => x.Name).Skip(skip).Take(Math.Max(1, take))
                            .ToList();
        }

        public async Task Update(Exercise exercise)
        {
            if (!UserContext.CanManageAllExercises())
                throw new SecurityException();

            var equipment = exercise.Equipment.ToLower();
            var existingEquipment = ExerciseRepository.GetAll().Where(x => x.Equipment.ToLower() == equipment).Any();
            if (!existingEquipment)
                throw new ArgumentException("You must use an existing piece of equipment");

            await ExerciseRepository.Update(exercise);
        }
    }
}
