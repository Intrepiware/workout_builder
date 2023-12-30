namespace WorkoutBuilder.Models
{
    public class WorkoutListItemModel
    {
        public long Id { get; set; }
        public string PublicId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsFavorite { get; set; }
        public string Name { get; set; }
    }
}
