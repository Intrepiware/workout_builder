using Microsoft.EntityFrameworkCore;

namespace WorkoutBuilder.Services.Impl
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext Context;
        public Repository(DbContext dbContext)
        {
            Context = dbContext;
        }

        public async Task Add(TEntity entity)
        {
            await Context.Set<TEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task Delete(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();
        }

        public IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().AsQueryable();
        }

        public async Task<TEntity> GetById(params object[] id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public async Task Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
            await Context.SaveChangesAsync();
        }
    }
}
