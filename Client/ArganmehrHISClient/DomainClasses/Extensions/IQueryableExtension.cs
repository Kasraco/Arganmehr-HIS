using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using DomainClasses;
namespace DomainClasses.Extensions
{
    public static class IQueryableExtension
    {
    
        public static IQueryable<T> Expand<T>(this IQueryable<T> query, string path) where T : BaseEntity
        {
        

            return query.Include(path);
        }

      
        public static IQueryable<T> Expand<T, TProperty>(this IQueryable<T> query, Expression<Func<T, TProperty>> path) where T : BaseEntity
        {
          

            return query.Include(path);
        }


    }
}
