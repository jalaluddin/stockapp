using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace StockMarketSharedLibrary
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Entity
    {
        protected DbContext context;
        protected DbSet<TEntity> dbSet;
        protected IDbCommandExecutionService dbCommandExecutionService;
        protected IDbCommandFactory dbCommandFactory;

        public GenericRepository(DbContext context, IDbCommandExecutionService dbCommandExecutionService, 
            IDbCommandFactory dbCommandFactory)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
            this.dbCommandExecutionService = dbCommandExecutionService;
            this.dbCommandFactory = dbCommandFactory;
        }

        public virtual IEnumerable<TEntity> Get(
            out int total, out int totalDisplay,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<TEntity> query = dbSet;
            total = query.Count();
            totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).Skip((pageIndex  - 1)* pageSize).Take(pageSize).ToList();
            }
            else
            {
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public virtual IEnumerable<TEntity> GetDynamic(out int total, out int totalDisplay, Expression<Func<TEntity, bool>> filter = null, 
            string orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<TEntity> query = dbSet;
            total = query.Count();
            totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public virtual IEnumerable<TEntity> GetDynamic(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return query.OrderBy(orderBy).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            int count = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                count = query.Count();
            }
            return count;
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual Dictionary<string, object> ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters,
            Dictionary<string, object> outParameters)
        {
            DbCommand command = dbCommandFactory.CreateCommand(storedProcedureName);
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    command.Parameters.Add(dbCommandFactory.CreateParameter(item.Key, item.Value));
                }
            }

            if(outParameters != null)
            {
                foreach(var item in outParameters)
                {
                    command.Parameters.Add(dbCommandFactory.CreateOutputParameter(item.Key, item.Value.GetType()));
                }
            }

            int effect = dbCommandExecutionService.ExecuteCommand(command);

            Dictionary<string, object> result = null;
            if(outParameters != null)
            {
                result = new Dictionary<string, object>();
                foreach(var item in outParameters)
                {
                    result.Add(item.Key, command.Parameters[item.Key].Value);
                }
            }

            return result;
        }

        public virtual List<TReturn> QueryWithStoredProcedure<TReturn>(string storedProcedureName, Dictionary<string, object> parameters) where TReturn : Entity
        {
            DbCommand command = dbCommandFactory.CreateCommand(storedProcedureName);
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    command.Parameters.Add(dbCommandFactory.CreateParameter(item.Key, item.Value));
                }
            }

            return dbCommandExecutionService.ExecuteQuery<TReturn>(command).ToList<TReturn>();
        }

        public virtual List<TReturn> QueryWithStoredProcedure<TReturn>(string storedProcedureName, Dictionary<string, object> parameters,
            Dictionary<string, Type> outParameters, out Dictionary<string, object> outValues) where TReturn : Entity
        {
            DbCommand command = dbCommandFactory.CreateCommand(storedProcedureName);
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    command.Parameters.Add(dbCommandFactory.CreateParameter(item.Key, item.Value));
                }
            }

            if (outParameters != null)
            {
                foreach (var item in outParameters)
                {
                    command.Parameters.Add(dbCommandFactory.CreateOutputParameter(item.Key, item.Value));
                }
            }

            var result = dbCommandExecutionService.ExecuteQuery<TReturn>(command).ToList<TReturn>();

            if (outParameters != null)
            {
                outValues = new Dictionary<string, object>();
                foreach (var item in outParameters)
                {
                    outValues.Add(item.Key, command.Parameters[item.Key].Value);
                }
            }
            else
                outValues = null;

            return result;
        }
    }
}
