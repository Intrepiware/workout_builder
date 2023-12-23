using System.Reflection;

namespace WorkoutBuilder.Services.Tests.TestUtilities
{
    public class TestRepo<T> : IRepository<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public List<T> DeletedItems { get; protected set; } = new List<T>();
        public List<T> AddedItems { get; protected set; } = new List<T>();
        public List<T> UpdatedItems { get; protected set; } = new List<T>();

        public List<long> DeletedIds => DeletedItems.Select(x => GetId(x).Value).ToList();
        public List<long> UpdatedIds => UpdatedItems.Select(x => GetId(x).Value).ToList();
        public List<long> AddedIds => AddedItems.Select(x => GetId(x).Value).ToList();

        public TestRepo() { }

        public TestRepo(IEnumerable<T> entities) => Data.AddRange(entities);

        public Task Add(T entity)
        {
            Data.Add(entity);
            AddedItems.Add(entity);
            return Task.CompletedTask;
        }

        public Task Delete(T entity)
        {
            Data.Remove(entity);
            DeletedItems.Add(entity);
            return Task.CompletedTask;
        }

        public IQueryable<T> GetAll() => Data.AsQueryable();

        public Task<T> GetById(params object[] id)
        {
            var item = Data.SingleOrDefault(x => GetId(x) != null && GetId(x).Value == (long)id[0]);
            return Task.FromResult(item);
        }

        public Task Update(T entity)
        {
            UpdatedItems.Add(entity);
            return Task.CompletedTask;
        }

        private long? GetId(T entity)
        {
            var type = entity.GetType();
            var property = type.GetProperty("Id");
            return (long)property?.GetValue(entity);
        }
    }
}