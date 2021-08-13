﻿using ProjectStone_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectStone_DataAccess.Data;

namespace ProjectStone_DataAccess.Repository
{
    /// <summary>
    /// This class essentially extends the use of AppDbContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        // ApplicationDbContext is needed here.
        private readonly ApplicationDbContext _db;

        // Internal DBSet for directly making changes.
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            // Set AppDbContext.
            _db = db;
            // Init dbSet to set a generic class with db set from _db.
            dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            // dbSet will have all the methods in, for example, ProductController when we bring up the intellisense in "Product."
            dbSet.Add(entity);
        }

        public T Find(int id)
        {
            // This is essentially "_db.Product.Find(id);", the purpose is to instance the calls of dbset.
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, bool isTracking = true)
        {
            // Query to build upon.
            IQueryable<T> query = dbSet;

            // Check for a "Where" filter.
            if (filter != null)
            {
                // Apply Where clause if there is a filter.
                query = query.Where(filter);
            }
            
            // Check for "IncludeProperties".
            if (includeProperties != null)
            {
                // Remove any empty entities if any are found.
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Include the properties in the query.
                    query = query.Include(includeProp);
                }
                
            }
            
            // Check for an "OrderBy" filter
            if (orderBy != null)
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
            IQueryable<T> query = dbSet;

            // Check for a "Where" filter.
            if (filter != null)
            {
                // Apply Where clause if there is a filter.
                query = query.Where(filter);
            }
            
            // Check for "IncludeProperties".
            if (includeProperties != null)
            {
                // Remove any empty entities if any are found.
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Include the properties in the query.
                    query = query.Include(includeProp);
                }
                
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

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Save()
        {
            // We will, for certain, be using AppDbContext to save all changes to the DB and pushing them through.
            _db.SaveChanges();
        }
    }
}