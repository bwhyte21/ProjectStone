using Microsoft.EntityFrameworkCore;
using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectStone_DataAccess.Repository;

/// <summary>
/// This class essentially extends the use of AppDbContext
/// </summary>
/// <typeparam name="T"></typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    // ApplicationDbContext is needed here.
    private readonly ApplicationDbContext _db;

    // Internal DBSet for directly making changes.
    internal DbSet<T> DbSet;

    public Repository(ApplicationDbContext db)
    {
        // Set AppDbContext.
        _db = db;
        // Init dbSet to set a generic class with db set from _db.
        DbSet = _db.Set<T>();
    }

    public T Find(int id)
    {
        // This is essentially "_db.Product.Find(id);", the purpose is to instance the calls of dbset.
        return DbSet.Find(id);
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, bool isTracking = true)
    {
        // Query to build upon.
        IQueryable<T> query = DbSet;

        // Check for a "Where" filter.
        if (filter is not null)
        {
            // Apply Where clause if there is a filter.
            query = query.Where(filter);
        }

        // Check for "IncludeProperties".
        if (includeProperties is not null)
        {
            // Remove any empty entities if any are found.
            query = includeProperties.Split(new[]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProp) => current.Include(includeProp));
        }

        // Check for an "OrderBy" filter
        if (orderBy is not null)
        {
            // Set the query to be ordered via the parameter.
            query = orderBy(query);
        }

        // Check if tracking is disabled.
        if (!isTracking)
        {
            // Disable tracking if false.
            query = query.AsNoTracking();
        }

        // Converting this query to a list will be executed due to differed execution.
        return query.ToList();
    }

    public T FirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null, bool isTracking = true)
    {
        // Similar to GetAll()

        // Query to build upon.
        IQueryable<T> query = DbSet;

        // Check for a "Where" filter.
        if (filter is not null)
        {
            // Apply Where clause if there is a filter.
            query = query.Where(filter);
        }

        // Check for "IncludeProperties".
        if (includeProperties is not null)
        {
            // Remove any empty entities if any are found.
            query = includeProperties.Split(new[]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProp) => current.Include(includeProp));
        }

        // Check if tracking is disabled.
        if (!isTracking)
        {
            // Disable tracking if false.
            query = query.AsNoTracking();
        }

        // Since we only want one object, we will call FirstOrDefault()
        return query.FirstOrDefault();
    }

    public void Add(T entity)
    {
        // dbSet will have all the methods in, for example, ProductController when we bring up the intellisense in "Product."
        DbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity)
    {
        DbSet.RemoveRange(entity);
    }

    public void Save()
    {
        // We will, for certain, be using AppDbContext to save all changes to the DB and pushing them through.
        _db.SaveChanges();
    }
}