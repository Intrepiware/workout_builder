namespace WorkoutBuilder.Services.Impl
{
    public class FakeEmailService : IEmailService
    {
        public void Send(string toAddress, string subject, string body)
        {
            Directory.CreateDirectory(@"C:\temp");
            Directory.CreateDirectory(@"C:\temp\email");
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd HH.mm.ss} - [{toAddress}] {subject}.html";

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            File.WriteAllText(@$"C:\temp\email\{fileName}", body);
        }
    }
}
