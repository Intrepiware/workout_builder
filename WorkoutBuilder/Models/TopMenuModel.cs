namespace WorkoutBuilder.Models
{
    public class TopMenuModel
    {
        public MenuItemModel Home { get; set; }
        public MenuItemModel TimingCalc { get; set; }
        public MenuItemModel Contact { get; set; }
        public MenuItemModel? Logout { get; set; }
        public MenuItemModel? Login { get; set; }
        public MenuItemModel? SignUp { get; set; }
        public MenuItemModel? Workouts { get; set; }
    }

    public class MenuItemModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
    }
}
