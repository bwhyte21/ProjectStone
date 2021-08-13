using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectStone_DataAccess.Repository.IRepository
{
    /// <summary>
    /// This is our Generic interface for our generic repository, Repository.cs, to be used
    /// throughout the project for Dependency Injection.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// C# Generic method here! When we create an interface on SubCategory, for example, then
        /// the "T" as the return type will represent the "SubCategory" model that we have.
        /// </summary>
        /// <param name="id">Gets the record id of what we want to find and returns an object of the target(T)</param>
        /// <returns>An object of T</returns>
        T Find(int id);

        /// <summary>
        /// Get all will return more than one record of the target, i.e; "SubCategory".
        /// </summary>
        /// <param name="filter">Null by default if there are no "where" conditions when getting IEnumerables(i.e; _db.Product.Include(u => u.Category).Include(u => u.SubCategory))</param>
        /// <param name="orderBy">Null by default if there are no "OrderBy" conditions.</param>
        /// <param name="includeProperties">Null by default if there are no Properties included(i.e; _db.Product.Include(u=>u.Category))</param>
        /// <param name="isTracking">By default, all queries run using EF are being tracked. Set to false if you do not wish to track.</param>
        /// <returns>A list of generic objects, T.</returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedEnumerable<T>> orderBy = null, string includeProperties = null, bool isTracking = true);

        /// <summary>
        /// Does not need an OrderBy because we're only looking for one record. The first or defaulted target.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includeProperties"></param>
        /// <param name="isTracking"></param>
        /// <returns>The first or default T</returns>
        T FirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null, bool isTracking = true);

        /// <summary>
        /// Add any generic object (T)
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// Remove any generic object (T)
        /// </summary>
        /// <param name="entity"></param>
        void Remove(T entity);

        /// <summary>
        /// Save the changes to the DB.
        /// </summary>
        void Save();
    }
}