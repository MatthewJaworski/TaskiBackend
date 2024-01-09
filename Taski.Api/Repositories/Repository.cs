using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Taski.Api.Data;
using Taski.Api.Entities;


namespace Taski.Api.Repositiories;

public class Repository<T> : IRepository<T> where T : class
{
  private DbContext dbContext;

  public Repository(IServiceProvider serviceProvider)
  {
    dbContext = new TaskiAppContext(serviceProvider.GetRequiredService<DbContextOptions<TaskiAppContext>>());

  }

  public async Task<IReadOnlyCollection<T>> GetAllAsync()
  {
    return await dbContext.Set<T>().ToListAsync();
  }
  public IQueryable<T> GetAll()
  {
    return dbContext.Set<T>();
  }
  public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
  {
    return await dbContext.Set<T>().Where(filter).ToListAsync();
  }

  public async Task<T> GetAsync(Guid id)
  {
    return await dbContext.Set<T>().FindAsync(id);
  }

  public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
  {
    return await dbContext.Set<T>().SingleOrDefaultAsync(filter);
  }



  public async Task CreateAsync(T item)
  {
    if (item == null)
    {
      throw new ArgumentNullException(nameof(item));
    }

    dbContext.Set<T>().Add(item);
    await dbContext.SaveChangesAsync();
  }

  public async Task UpdateAsync(T item)
  {
    if (item == null)
    {
      throw new ArgumentNullException(nameof(item));
    }

    dbContext.Set<T>().Update(item);
    await dbContext.SaveChangesAsync();
  }

  public async Task RemoveAsync(Guid id)
  {
    var item = await dbContext.Set<T>().FindAsync(id);
    if (item != null)
    {
      dbContext.Set<T>().Remove(item);
      await dbContext.SaveChangesAsync();
    }
  }

}