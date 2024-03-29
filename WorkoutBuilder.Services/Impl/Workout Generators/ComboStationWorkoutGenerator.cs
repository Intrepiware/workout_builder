﻿using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl.Workout_Generators
{
    public class ComboStationWorkoutGenerator : IWorkoutGenerator
    {
        public IRandomize Randomizer { init; protected get; } = null!;
        public IRepository<Exercise> ExerciseRepository { init; protected get; } = null!;

        public WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request)
        {
            var equipment = request.Equipment ?? new List<string>();
            var exercises = ExerciseRepository.GetAll().Where(x => equipment.Contains(x.Equipment)).ToList();
            var allEquipment = exercises.Select(x => x.Equipment).Distinct().ToList();
            var addedExerciseIds = new List<long>();
            const int MaxIterations = 1000;

            var timing = request.Timing;
            var focus = request.Focus ?? Randomizer.GetRandomItem(new[] { Models.Focus.Cardio, Models.Focus.Hybrid, Models.Focus.Strength });

            double cardio = 0, strength = .8;

            switch (focus)
            {
                case Models.Focus.Strength:
                    cardio = 0;
                    break;

                case Models.Focus.Hybrid:
                    cardio = 0.4;
                    break;

                case Models.Focus.Cardio:
                default:
                    cardio = 0.6;
                    break;
            }

            var output = new WorkoutGenerationResponseModel
            {
                Focus = focus.ToString(),
                Name = timing.Name,
                Notes = timing.Notes,
                Stations = timing.Stations,
                Timing = timing.StationTiming,
                Exercises = new List<WorkoutGenerationExerciseModel>(),
            };

            var iterations = 0;
            while (output.Exercises.Count < output.Stations && iterations++ < MaxIterations)
            {
                // Don't use more than 15 different pieces of equipment per workout
                var usedEquipment = output.Exercises.Select(x => x.Equipment).Distinct();
                var allowedEquipment = usedEquipment.Count() >= 15 ? usedEquipment : allEquipment;

                var exercise1 = GetNext(exercises, null, cardio, strength);
                if (exercise1 == null || !allowedEquipment.Contains(exercise1.Equipment))
                    continue;

                // The second exercise should use the same equipment or bodyweight, with a strong preference for the same equipment
                var nextEquipment = Randomizer.NextDouble() < .8 ? exercise1.Equipment : "bodyweight";
                var exercise2 = GetNext(exercises, nextEquipment, cardio, strength);

                // Include in the workout if the two exercises are different, and they aren't already in the workout
                if (exercise1 != null && exercise2 != null && exercise1.Id != exercise2.Id
                    && !addedExerciseIds.Contains(exercise1.Id) && !addedExerciseIds.Contains(exercise2.Id))
                {
                    var station = 1 + output.Exercises.Count / 2;

                    output.Exercises.Add(new WorkoutGenerationExerciseModel
                    {
                        Equipment = exercise1.Equipment,
                        Exercise = exercise1.Name,
                        YoutubeUrl = exercise1.YoutubeUrl,
                        Focus = ((Models.Focus)exercise1.FocusId).ToString(),
                        Notes = exercise1.Notes,
                        Station = $"{station}A"
                    });
                    addedExerciseIds.Add(exercise1.Id);

                    output.Exercises.Add(new WorkoutGenerationExerciseModel
                    {
                        Equipment = exercise2.Equipment,
                        Exercise = exercise2.Name,
                        YoutubeUrl = exercise2.YoutubeUrl,
                        Focus = ((Models.Focus)exercise2.FocusId).ToString(),
                        Notes = exercise2.Notes,
                        Station = $"{station}B"
                    });
                    addedExerciseIds.Add(exercise2.Id);
                }
            }
            return output;
        }

        private Exercise GetNext(IEnumerable<Exercise> exercises, string equipmentItem, double cardio, double strength)
        {
            var rand = Randomizer.NextDouble();
            Models.Focus exerciseFocus;
            if (rand < cardio)
                exerciseFocus = Models.Focus.Cardio;
            else if (rand < strength)
                exerciseFocus = Models.Focus.Strength;
            else
                exerciseFocus = Models.Focus.Abs;

            var filteredExercises = exercises.Where(x => x.FocusId == (byte)exerciseFocus
                && (equipmentItem == null || x.Equipment.Equals(equipmentItem, StringComparison.OrdinalIgnoreCase)));

            var exercise = Randomizer.GetRandomItem(filteredExercises);
            return exercise;
        }
    }
}