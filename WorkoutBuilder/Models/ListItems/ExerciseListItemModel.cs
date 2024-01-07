namespace WorkoutBuilder.Models.ListItems
{
    public class ExerciseListItemModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Focus { get; set; }
        public string? EditUrl { get; set; }
    }
}
