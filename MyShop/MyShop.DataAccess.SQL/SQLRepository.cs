using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.DataAccess.SQL
{
    
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {
        //for the class to work, we need to inject into the DataContext class
        internal DataContext context;
        //we also need a way to map the underline product to the underline the product table itself
        //we do that using a couple special internal context commmands
        internal DbSet<T> dbSet; //underline table we need to access


        /// <summary>
        /// Constructor needs to allow to passa a DataContext
        /// and also set the underline table, rerecing the context and calling the set command passing in the model we want to work against
        /// </summary>
        /// <param name="context"></param>
        public SQLRepository(DataContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        /// <summary>
        /// to return the dbset
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Collection()
        {
            return dbSet;
        }

        /// <summary>
        /// to save the changes on the underline table
        /// </summary>
        public void Commit()
        {
            context.SaveChanges();
        }

        /// <summary>
        /// First need to find the object based on the methods, using the classes own find method
        /// Then check the state, if not connected, the objected will be attached and then removed
        /// </summary>
        /// <param name="Id"></param>
        public void Delete(string Id)
        {
            var t = Find(Id);
            if (context.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }

            dbSet.Remove(t);
        }

        public T Find(string Id)
        {
            //dbSet type has its own Find method
            return dbSet.Find(Id);
        }

        /// <summary>
        /// exposes its own insert method in the DBSet type
        /// </summary>
        /// <param name="t"></param>
        public void Insert(T t)
        {
            dbSet.Add(t);
        }

        /// <summary>
        /// Slighly more complex, first we need to attach our model
        /// Then set that entry to a state of modified
        /// </summary>
        /// <param name="t"></param>
        public void Update(T t)
        {
            dbSet.Attach(t);
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
