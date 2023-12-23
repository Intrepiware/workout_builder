namespace WorkoutBuilder.Services
{
    public interface IUrlBuilder
    {
        string Action(string action, string controller, object values);
    }
}
