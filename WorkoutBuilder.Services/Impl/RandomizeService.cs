namespace WorkoutBuilder.Services.Impl
{
    public class RandomizeService : IRandomize
    {

        // https://stackoverflow.com/a/38530913
        private static int _tracker = 0;

        private static ThreadLocal<Random> _random = new ThreadLocal<Random>(() => {
            var seed = (int)(Environment.TickCount & 0xFFFFFF00 | (byte)(Interlocked.Increment(ref _tracker) % 255));
            var random = new Random(seed);
            return random;
        });

        public T GetRandomItem<T>(IEnumerable<T> items) => items.OrderBy(x => _random.Value.NextDouble()).FirstOrDefault();

        public double NextDouble() => _random.Value.NextDouble();
    }
}
