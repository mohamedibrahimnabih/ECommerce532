using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce532.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;// = new();
    private readonly DbSet<T> _db;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _db = _context.Set<T>();
    }

    // T
    // CRUD
    public async Task<bool> CreateAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            await _db.AddAsync(entity, ct);

            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public bool Update(T entity)
    {
        try
        {
            _db.Update(entity);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public bool Delete(T entity)
    {
        try
        {
            _db.Remove(entity);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 0;
        }
    }


    public IQueryable<T> Get(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true)
    {
        var categories = _db.AsQueryable();

        if (expression is not null)
            categories = categories.Where(expression);

        if(includes is not null && includes.Any())
        {
            foreach (var item in includes)
                categories = categories.Include(item);
        }

        if(!tracked)
            categories = categories.AsNoTracking();

        //_db.T.Where(e => e.Status);

        return categories;
    }

    public T? GetOne(

        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true)
    {
        return Get(expression, includes, tracked).FirstOrDefault();
    }
}
