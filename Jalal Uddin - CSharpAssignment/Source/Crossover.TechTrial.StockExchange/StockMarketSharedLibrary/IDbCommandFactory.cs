using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StockMarketSharedLibrary
{
    public interface IDbCommandFactory
    {
        DbCommand CreateUpdateCommand(Entity item);
        DbCommand CreateDeleteCommand(Entity item);
        DbCommand CreateMultiDeleteCommand(ICollection<Entity> items);
        DbCommand CreateMultiDeleteCommand(Type entityType, ICollection<object> itemIDs);
        DbCommand CreateGetCommand(Type entityType, object id);
        DbCommand CreateCommand(string commandText, params object[] parameters);
        DbParameter CreateParameter(string name, object value);
        DbParameter CreateParameterFromNonPublicField(string name, object instance, string propertyName);
        DbParameter CreateOutputParameter(string name, DbType dbType);
        DbParameter CreateOutputParameter(string name, Type type);
        DbCommand CreateTotalCountCommand(Type entityType);
        DbCommand CreateTotalCountCommand(Type entityType, object searchItem);
        DbCommand CreateTotalCountOrCommand(Type entityType, object searchItem);
        DbCommand CreateTotalCountLiteralOrCommand(Type entityType, object searchItem);
        DbCommand CreateGetAllCommand(Type entityType);
        DbCommand CreateGetAllPagedCommand(Type entityType, int pageIndex, int pageSize, object searchItem, ICollection<SortElement> sortOn);
        DbCommand CreateGetAllPagedOrCommand(Type entityType, int pageIndex, int pageSize, object searchItem, ICollection<SortElement> sortOn);
        DbCommand CreateGetAllPagedLiteralOrCommand(Type entityType, int pageIndex, int pageSize, object searchItem, ICollection<SortElement> sortOn);
        Queue<DbCommand> CreateInsertCommands(Entity item);
        Queue<DbCommand> CreateUpdateCommands(Entity item);
        Queue<DbCommand> CreateDeleteCommands(Entity item);
        ICollection<PropertyInfo> GetNotBaseProperties(Type itemItem, Type baseType);
        bool IsSimpleParameterType(Type parameterType);
        DbCommand AddSearchCommandParameters(object searchItem, DbCommand command);
        DbCommand AddSortOrderCommandParameter(Type entityType, ICollection<SortElement> sortOn, DbCommand command);
        DbCommand AddCommandParameters(Entity item, DbCommand command);
    }
}
