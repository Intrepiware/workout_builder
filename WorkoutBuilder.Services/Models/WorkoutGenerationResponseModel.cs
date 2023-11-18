namespace WorkoutBuilder.Services.Models
{

    public class WorkoutGenerationResponseModel
    {
        public string Name { get; set; }
        public string Focus { get; set; }
        public int Stations { get; set; }
        public string Timing { get; set; }
        public string? Notes { get; set; }
        public List<WorkoutGenerationExerciseModel> Exercises { get; set; }
    }

    public class WorkoutGenerationExerciseModel
    {
        public string Station { get; set; }
        public string Exercise { get; set; }
        public string Focus { get; set; }
        public string Equipment { get; set; }
        public string? Notes { get; set; }
    }

}
