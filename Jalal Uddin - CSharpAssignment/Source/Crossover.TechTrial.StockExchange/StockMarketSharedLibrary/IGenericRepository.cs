using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace StockMarketSharedLibrary
{
    public interface IGenericRepository<TEntity>
     where TEntity : Entity
    {
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        Dictionary<string, object> ExecuteStoredProcedure(string storedProcedureName, System.Collections.Generic.Dictionary<string, object> parameters, System.Collections.Generic.Dictionary<string, object> outParameters);
        IEnumerable<TEntity> Get(out int total, out int totalDisplay, Expression<Func<TEntity, bool>> filter = null, Func<System.Linq.IQueryable<TEntity>, System.Linq.IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10);
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        IEnumerable<TEntity> GetDynamic(out int total, out int totalDisplay, Expression<Func<TEntity, bool>> filter = null, string orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10);
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        IEnumerable<TEntity> GetDynamic(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, string includeProperties = "");
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        List<TReturn> QueryWithStoredProcedure<TReturn>(string storedProcedureName, System.Collections.Generic.Dictionary<string, object> parameters) where TReturn : Entity;
        List<TReturn> QueryWithStoredProcedure<TReturn>(string storedProcedureName, System.Collections.Generic.Dictionary<string, object> parameters, System.Collections.Generic.Dictionary<string, Type> outParameters, out System.Collections.Generic.Dictionary<string, object> outValues) where TReturn : Entity;
        void Update(TEntity entityToUpdate);
    }
}
