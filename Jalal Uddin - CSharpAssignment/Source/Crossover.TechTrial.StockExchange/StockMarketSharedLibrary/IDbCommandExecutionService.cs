using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarketSharedLibrary
{
    public interface IDbCommandExecutionService
    {
        int ExecuteCommand(DbCommand command);
        int ExecuteCommand(Queue<DbCommand> commands);
        IEnumerable<T> ExecuteQuery<T>(DbCommand command) where T : Entity;
        IEnumerable ExecuteQuery(Type type, DbCommand command);
        IEnumerable<Entity> ExecuteQuery(Entity dataContainer, DbCommand command);
        T ExecuteScalar<T>(DbCommand command);
    }
}
