using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StockMarketSharedLibrary
{
    public class SqlCommandFactory : IDbCommandFactory
    {
        #region Messages

        private string _invalidEntityMessage = "Provided entity type is invalid";
        private string _invalidCollectionMessage = "Provided collection to create command is either null or empty, can't create command";

        #endregion

        private string _connectionString;

        public SqlCommandFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }
        
        public virtual ICollection<PropertyInfo> GetNotBaseProperties(Type itemItem, Type baseType)
        {
            PropertyInfo[] properties = itemItem.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] baseProperties = baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> unCommonProperties = new List<PropertyInfo>();
            foreach (var property in properties)
            {
                if (!baseProperties.Any<PropertyInfo>(x => x.Name == property.Name) || property.Name == "ID")
                    unCommonProperties.Add(property);
            }

            return unCommonProperties;
        }

        public virtual DbCommand CreateUpdateCommand(Entity item)
        {
            if (item != null)
            {
                Type entityType = item.GetType();
                PropertyInfo[] properties = entityType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                properties = properties.Where(x => x.GetCustomAttribute<WriteIgnoreAttribute>() == null).ToArray();

                string commandText = String.Format("{0}_Edit{1}", entityType.Name, entityType.Name);
                List<object> parameters = new List<object>();
                foreach (var property in properties)
                {
                    if (IsSimpleParameterType(property.PropertyType))
                        parameters.Add(new SqlParameter(property.Name, CorrectSqlDateTime(property.GetValue(item))));
                    else
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(Entity).IsAssignableFrom(childPropertyType))
                            {
                                parameters.Add(new SqlParameter(property.Name + "ID",
                                    ((Entity)childPropertyValue).ID));
                            }
                        }
                        else if (typeof(Entity).IsAssignableFrom(property.PropertyType))
                            parameters.Add(new SqlParameter(property.Name + "ID", null));
                    }
                }

                DbCommand command = this.CreateCommand(commandText, parameters.ToArray());

                return command;
            }
            else
                throw new ArgumentException("Provided collection to create update command is either null or empty, can't create command");
        }

        public virtual Queue<DbCommand> CreateInsertCommands(Entity item)
        {
            if (item != null)
            {
                Queue<DbCommand> queue = new Queue<DbCommand>();
                queue = InsertCommandRecursiveSolve(item.GetType(), item, queue);
                return queue;
            }
            else
                throw new ArgumentNullException("Provided item to create insert command is null, can't create command");
        }

        public virtual Queue<DbCommand> CreateUpdateCommands(Entity item)
        {
            if (item != null)
            {
                Queue<DbCommand> queue = new Queue<DbCommand>();
                queue = UpdateCommandRecursiveSolve(item.GetType(), item, queue);
                return queue;
            }
            else
                throw new ArgumentNullException("Provided item to create insert command is null, can't create command");
        }

        public virtual DbCommand CreateDeleteCommand(Entity item)
        {
            if (item != null)
            {
                Type entityType = item.GetType();
                PropertyInfo idProperty = entityType.GetProperty("ID");

                string commandText = String.Format("{0}_Delete{1}", entityType.Name, entityType.Name);
                DbCommand command = this.CreateCommand(commandText, new SqlParameter("ID", idProperty.GetValue(item)));

                return command;
            }
            else
                throw new ArgumentException("Provided collection to create delete command is either null or empty, can't create command");
        }

        public virtual Queue<DbCommand> CreateDeleteCommands(Entity item)
        {
            if (item != null)
            {
                Queue<DbCommand> queue = new Queue<DbCommand>();
                queue = DeleteCommandRecursiveSolve(item.GetType(), item, queue);
                return queue;
            }
            else
                throw new ArgumentNullException("Provided item to create insert command is null, can't create command");
        }

        public virtual DbCommand CreateMultiDeleteCommand(ICollection<Entity> items)
        {
            if (items != null && items.Count > 0)
            {
                Type entityType = items.First<Entity>().GetType();
                PropertyInfo idProperty = entityType.GetProperty("ID");

                StringBuilder xmlString = new StringBuilder();
                foreach (var item in items)
                {
                    xmlString.AppendFormat("<ID>{0}</ID>", idProperty.GetValue(item));
                }

                string commandText = String.Format("{0}_MultiDelete{1}", entityType.Name, entityType.Name);
                DbCommand command = this.CreateCommand(commandText, new SqlParameter("IDs", xmlString.ToString()));

                return command;
            }
            else
                throw new ArgumentException("Provided collection to create multi delete command is either null or empty, can't create command");
        }

        public virtual DbCommand CreateMultiDeleteCommand(Type entityType, ICollection<object> itemIDs)
        {
            if (entityType == null)
            {
                throw new ArgumentException(_invalidEntityMessage);
            }

            if (itemIDs != null && itemIDs.Count > 0)
            {
                StringBuilder xmlString = new StringBuilder();
                foreach (var itemID in itemIDs)
                {
                    xmlString.AppendFormat("<ID>{0}</ID>", itemID);
                }

                string commandText = String.Format("{0}_MultiDelete{1}", entityType.Name, entityType.Name);
                DbCommand command = CreateCommand(commandText, new SqlParameter("IDs", xmlString.ToString()));

                return command;
            }
            else
                throw new ArgumentException(_invalidCollectionMessage);
        }

        public virtual DbCommand CreateGetCommand(Type entityType, object id)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            if (!typeof(Entity).IsAssignableFrom(entityType))
                throw new ArgumentException(_invalidEntityMessage);
            if(id == null)
                throw new ArgumentNullException("id");

            string commandText = String.Format("{0}_Get{1}", entityType.Name, entityType.Name);
            DbCommand command = CreateCommand(commandText, new SqlParameter("ID", id));

            return command;
        }

        public virtual DbCommand CreateTotalCountCommand(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            if (!typeof(Entity).IsAssignableFrom(entityType))
                throw new ArgumentException(_invalidEntityMessage);

            string commandText = String.Format("{0}_TotalCount{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText, new object[0]);

            SqlParameter returnValue = new SqlParameter("TotalCount", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            command.Parameters.Add(returnValue);

            return command;
        }

        public virtual DbCommand CreateTotalCountCommand(Type entityType, object searchItem)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            
            string commandText = String.Format("{0}_TotalCountWithSearch{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText, new object[0]);

            command = AddSearchCommandParameters(searchItem, command);

            SqlParameter returnValue = new SqlParameter("TotalCount", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            command.Parameters.Add(returnValue);

            return command;
        }

        public virtual DbCommand CreateTotalCountOrCommand(Type entityType, object searchItem)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            string commandText = String.Format("{0}_TotalCountWithSearchOr{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText, new object[0]);

            command = AddSearchCommandParameters(searchItem, command);

            SqlParameter returnValue = new SqlParameter("TotalCount", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            command.Parameters.Add(returnValue);

            return command;
        }

        public virtual DbCommand CreateTotalCountLiteralOrCommand(Type entityType, object searchItem)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            string commandText = String.Format("{0}_TotalCountWithSearchLiteralOr{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText, new object[0]);

            command = AddSearchCommandParameters(searchItem, command);

            SqlParameter returnValue = new SqlParameter("TotalCount", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            command.Parameters.Add(returnValue);

            return command;
        }

        public virtual DbCommand CreateGetAllCommand(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            if (!typeof(Entity).IsAssignableFrom(entityType))
                throw new ArgumentException(_invalidEntityMessage);

            string commandText = String.Format("{0}_GetAll{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText, new object[0]);

            return command;
        }

        public virtual DbCommand AddSortOrderCommandParameter(Type entityType, ICollection<SortElement> sortOn, DbCommand command)
        {
            PropertyInfo[] properties = entityType.GetProperties(BindingFlags.NonPublic 
                | BindingFlags.Public | BindingFlags.Instance);

            // If any sort instruction is passed, build a sort expression
            StringBuilder sortOrder = new StringBuilder();

            if (sortOn != null && sortOn.Count > 0)
            {
                int appendCount = 0;
                foreach (var sortItem in sortOn)
                {
                    if (properties.Any(x => x.Name == sortItem.ColumnName))
                    {
                        if (appendCount > 0)
                            sortOrder.Append(", ");

                        sortOrder.Append(string.Format("{0} {1}", sortItem.ColumnName,
                            sortItem.Order == SortOrder.Descending ? "desc" : "asc"));

                        appendCount++;
                    }
                }
            }
            if (sortOrder.Length > 0)
                command.Parameters.Add(new SqlParameter("SortOrder", sortOrder.ToString()));
            else
                command.Parameters.Add(new SqlParameter("SortOrder", "ID ASC"));

            return command;
        }

        public virtual DbCommand AddSearchCommandParameters(object searchItem, DbCommand command)
        {
            // if any search item is passed, add search parameters
            if (searchItem != null)
            {
                var searchProperties = searchItem.GetType().GetProperties(BindingFlags.NonPublic |
                    BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in searchProperties)
                {
                    if (IsSimpleParameterType(property.PropertyType))
                    {
                        command.Parameters.Add(new SqlParameter(property.Name, CorrectSqlDateTime(property.GetValue(searchItem))));
                    }
                }
            }
            return command;
        }

        public virtual DbCommand AddCommandParameters(Entity item, DbCommand command)
        {
            // if any item is passed, add parameters
            if (item != null)
            {
                Type itemType = item.GetType();
                var entityProperties = itemType.GetProperties(BindingFlags.NonPublic |
                    BindingFlags.Public | BindingFlags.Instance).ToList();

                entityProperties = entityProperties.Where(x => x.GetCustomAttribute<WriteIgnoreAttribute>() == null).ToList();

                foreach (var property in entityProperties)
                {
                    if (IsSimpleParameterType(property.PropertyType))
                    {
                        command.Parameters.Add(new SqlParameter(property.Name, CorrectSqlDateTime(property.GetValue(item))));
                    }
                }
            }
            return command;
        }

        public virtual DbCommand CreateGetAllPagedCommand(Type entityType, int pageIndex, int pageSize, object searchItem, ICollection<SortElement> sortOn)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            

            string commandText = String.Format("{0}_GetAllPaged{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText,
                new SqlParameter("CurrentPageIndex", pageIndex),
                new SqlParameter("PageSize", pageSize));

            command = AddSortOrderCommandParameter(entityType, sortOn, command);

            command = AddSearchCommandParameters(searchItem, command);

            return command;
        }

        public virtual DbCommand CreateGetAllPagedOrCommand(Type entityType, int pageIndex, int pageSize, object searchItem, ICollection<SortElement> sortOn)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");


            string commandText = String.Format("{0}_GetAllPagedOr{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText,
                new SqlParameter("CurrentPageIndex", pageIndex),
                new SqlParameter("PageSize", pageSize));

            command = AddSortOrderCommandParameter(entityType, sortOn, command);

            command = AddSearchCommandParameters(searchItem, command);

            return command;
        }

        public virtual DbCommand CreateGetAllPagedLiteralOrCommand(Type entityType, int pageIndex, int pageSize, object searchItem, ICollection<SortElement> sortOn)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");


            string commandText = String.Format("{0}_GetAllPagedLiteralOr{1}", entityType.Name, entityType.Name);
            DbCommand command = this.CreateCommand(commandText,
                new SqlParameter("CurrentPageIndex", pageIndex),
                new SqlParameter("PageSize", pageSize));

            command = AddSortOrderCommandParameter(entityType, sortOn, command);

            command = AddSearchCommandParameters(searchItem, command);

            return command;
        }

        public virtual DbCommand CreateCommand(string commandText, params object[] parameters)
        {
            DbCommand command = new SqlCommand(commandText, new SqlConnection(this._connectionString));
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandTimeout = 18000;

            var dbParameters = new DbParameter[parameters.Length];
            if (parameters.All(p => p is DbParameter))
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    dbParameters[i] = (DbParameter)parameters[i];
                    if (((DbParameter)parameters[i]).Value == null)
                        dbParameters[i].Value = DBNull.Value;
                }
            }
            else if (!parameters.Any(p => p is DbParameter))
            {
                var parameterNames = new string[parameters.Length];
                var parameterSql = new string[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    parameterNames[i] = string.Format(CultureInfo.InvariantCulture, "p{0}", i);
                    dbParameters[i] = command.CreateParameter();
                    dbParameters[i].ParameterName = parameterNames[i];
                    dbParameters[i].Value = parameters[i] ?? DBNull.Value;
                    parameterSql[i] = "@" + parameterNames[i];
                }
                command.CommandText = string.Format(CultureInfo.InvariantCulture, command.CommandText, parameterSql);
            }
            else
            {
                throw new InvalidOperationException("Mixed DB parameters are not allowed.");
            }

            foreach (var dbParameter in dbParameters)
            {
                if (!command.Parameters.Contains(dbParameter.ParameterName))
                    command.Parameters.Add(dbParameter);
            }
            return command;
        }

        public virtual DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, CorrectSqlDateTime(value));
        }

        public virtual DbParameter CreateParameterFromNonPublicField(string name, object instance, string propertyName)
        {
            if (instance != null)
            {
                Type entityType = instance.GetType();
                PropertyInfo[] properties = entityType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in properties)
                {
                    if (IsSimpleParameterType(property.PropertyType) && property.Name == propertyName)
                        return new SqlParameter(property.Name, CorrectSqlDateTime(property.GetValue(instance)));
                }
            }
            return null;
        }
        
        public virtual DbParameter CreateOutputParameter(string name, DbType dbType)
        {
            SqlParameter outParam = new SqlParameter(name, CorrectSqlDateTime(dbType));
            outParam.Direction = ParameterDirection.Output;
            return outParam;
        }

        public virtual DbParameter CreateOutputParameter(string name, Type type)
        {
            SqlParameter outParam = new SqlParameter(name, GetDbTypeFromType(type));
            outParam.Direction = ParameterDirection.Output;
            return outParam;
        }

        public virtual bool IsSimpleParameterType(Type parameterType)
        {
            return (parameterType == typeof(string) || parameterType.IsValueType || parameterType.IsEnum);
        }

        protected virtual SqlDbType GetDbTypeFromType(Type type)
        {
            if (type == typeof(int))
                return SqlDbType.Int;
            else if (type == typeof(double))
                return SqlDbType.Decimal;
            else if (type == typeof(string))
                return SqlDbType.NVarChar;
            else if (type == typeof(DateTime))
                return SqlDbType.DateTime;
            else if (type == typeof(bool))
                return SqlDbType.Bit;
            else if (type == typeof(Guid))
                return SqlDbType.UniqueIdentifier;
            else if (type == typeof(char))
                return SqlDbType.NVarChar;
            else
                return SqlDbType.NVarChar;
        }

        protected virtual Queue<DbCommand> DeleteCommandRecursiveSolve(Type typeToSolve, Entity item, Queue<DbCommand> currentQueue)
        {
            if (item == null || item.IsTransient() || item.IsIgnored())
                return currentQueue;

            string commandText = String.Format("{0}_Delete{1}", typeToSolve.Name, typeToSolve.Name);
            ICollection<PropertyInfo> properties = null;

            if (typeToSolve.BaseType == typeof(Entity))
            {
                properties = typeToSolve.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            else
            {
                properties = GetNotBaseProperties(typeToSolve, typeToSolve.BaseType);
            }

            // If only one parameter was added, then there is no field in this child class except ID
            // so we don't need a separate table
            if (properties.Count > 1)
            {
                foreach (var property in properties)
                {
                    if (!IsSimpleParameterType(property.PropertyType))
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(IEnumerable).IsAssignableFrom(childPropertyType))
                            {
                                foreach (object o in (childPropertyValue as IEnumerable))
                                {
                                    if (o != null && o is Entity)
                                    {
                                        currentQueue = DeleteCommandRecursiveSolve(o.GetType(), (Entity)o, currentQueue);
                                    }
                                }
                            }
                        }
                    }
                }

                DbCommand command = this.CreateCommand(commandText, new SqlParameter("ID", item.ID));
                currentQueue.Enqueue(command);
                
                foreach (var property in properties)
                {
                    if (!IsSimpleParameterType(property.PropertyType))
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(Entity).IsAssignableFrom(childPropertyType))
                            {
                                currentQueue = DeleteCommandRecursiveSolve(childPropertyType, (Entity)childPropertyValue, currentQueue);
                            }
                        }
                    }
                }
            }

            if (typeToSolve.BaseType != typeof(Entity))
            {
                currentQueue = DeleteCommandRecursiveSolve(typeToSolve.BaseType, item, currentQueue);
            }

            return currentQueue;
        }

        protected virtual Queue<DbCommand> InsertCommandRecursiveSolve(Type typeToSolve, Entity item,
            Queue<DbCommand> currentQueue, params DbParameter[] extraParameters)
        {
            if (item == null || item.IsTransient() || item.IsIgnored())
                return currentQueue;

            string commandText = String.Format("{0}_Add{1}", typeToSolve.Name, typeToSolve.Name);
            List<object> parameters = new List<object>();
            ICollection<PropertyInfo> properties = null;

            if (typeToSolve.BaseType == typeof(Entity))
            {
                properties = typeToSolve.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                // This is needed to add foriegn key reference for solving collection type from previous step.
                parameters.AddRange(extraParameters);
            }
            else
            {
                // Solving the parent class first
                currentQueue = InsertCommandRecursiveSolve(typeToSolve.BaseType, item, currentQueue);
                // only taking properties that is not of parent class in consideration
                properties = GetNotBaseProperties(typeToSolve, typeToSolve.BaseType);
            }

            properties = properties.Where(x => x.GetCustomAttribute<WriteIgnoreAttribute>() == null).ToList();

            foreach (var property in properties)
            {
                if (IsSimpleParameterType(property.PropertyType) && property.CanWrite)
                    parameters.Add(new SqlParameter(property.Name, CorrectSqlDateTime(property.GetValue(item))));
            }

            // If only one parameter was added, then there is no field in this child class except ID
            // so we don't need a separate table
            if (parameters.Count > 1)
            {
                DbCommand command = CreateCommand(commandText, parameters.ToArray());

                // Iterating for complex properties that are not of collection type
                foreach (var property in properties)
                {
                    if (!IsSimpleParameterType(property.PropertyType))
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(Entity).IsAssignableFrom(childPropertyType))
                            {
                                command.Parameters.Add(new SqlParameter(property.Name + "ID",
                                    ((Entity)childPropertyValue).ID));

                                currentQueue = InsertCommandRecursiveSolve(childPropertyType, (Entity)childPropertyValue, currentQueue);
                            }
                        }
                        else if (typeof(Entity).IsAssignableFrom(property.PropertyType))
                            command.Parameters.Add(new SqlParameter(property.Name + "ID", null));
                    }
                }

                currentQueue.Enqueue(command);

                // Iterating for complex properties that of collection type
                foreach (var property in properties)
                {
                    if (!IsSimpleParameterType(property.PropertyType))
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(IEnumerable).IsAssignableFrom(childPropertyType))
                            {
                                foreach (object o in (childPropertyValue as IEnumerable))
                                {
                                    if (o != null && o is Entity)
                                    {
                                        currentQueue = InsertCommandRecursiveSolve(o.GetType(), (Entity)o, currentQueue,
                                            new SqlParameter(GetHigestBaseType(item.GetType()).Name + "ID", item.ID));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return currentQueue;
        }

        protected virtual Queue<DbCommand> UpdateCommandRecursiveSolve(Type typeToSolve, Entity item,
            Queue<DbCommand> currentQueue, params DbParameter[] extraParameters)
        {
            if (item == null || item.IsTransient() || item.IsIgnored())
                return currentQueue;

            string commandText = String.Format("{0}_Edit{1}", typeToSolve.Name, typeToSolve.Name);
            List<object> parameters = new List<object>();
            ICollection<PropertyInfo> properties = null;

            if (typeToSolve.BaseType == typeof(Entity))
            {
                properties = typeToSolve.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                // This is needed to add foriegn key reference for solving collection type from previous step.
                parameters.AddRange(extraParameters);
            }
            else
            {
                // Solving the parent class first
                currentQueue = UpdateCommandRecursiveSolve(typeToSolve.BaseType, item, currentQueue);
                // only taking properties that is not of parent class in consideration
                properties = GetNotBaseProperties(typeToSolve, typeToSolve.BaseType);
            }

            properties = properties.Where(x => x.GetCustomAttribute<WriteIgnoreAttribute>() == null).ToList();

            foreach (var property in properties)
            {
                if (IsSimpleParameterType(property.PropertyType) && property.CanWrite)
                    parameters.Add(new SqlParameter(property.Name, CorrectSqlDateTime(property.GetValue(item))));
            }

            // If only one parameter was added, then there is no field in this child class except ID
            // so we don't need a separate table
            if (parameters.Count > 1)
            {
                DbCommand command = CreateCommand(commandText, parameters.ToArray());

                // Iterating for complex properties that are not of collection type
                foreach (var property in properties)
                {
                    if (!IsSimpleParameterType(property.PropertyType))
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(Entity).IsAssignableFrom(childPropertyType))
                            {
                                command.Parameters.Add(new SqlParameter(property.Name + "ID",
                                    ((Entity)childPropertyValue).ID));

                                currentQueue = UpdateCommandRecursiveSolve(childPropertyType, (Entity)childPropertyValue, currentQueue);
                            }
                        }
                        else if(typeof(Entity).IsAssignableFrom(property.PropertyType))
                            command.Parameters.Add(new SqlParameter(property.Name + "ID", null));
                    }
                }

                currentQueue.Enqueue(command);

                // Iterating for complex properties that of collection type
                foreach (var property in properties)
                {
                    if (!IsSimpleParameterType(property.PropertyType))
                    {
                        var childPropertyValue = property.GetValue(item);
                        if (childPropertyValue != null)
                        {
                            Type childPropertyType = childPropertyValue.GetType();
                            if (typeof(IEnumerable).IsAssignableFrom(childPropertyType))
                            {
                                foreach (object o in (childPropertyValue as IEnumerable))
                                {
                                    if (o != null && o is Entity)
                                    {
                                        currentQueue = UpdateCommandRecursiveSolve(o.GetType(), (Entity)o, currentQueue,
                                            new SqlParameter(GetHigestBaseType(item.GetType()).Name + "ID", item.ID));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return currentQueue;
        }

        protected Type GetHigestBaseType(Type type)
        {
            while (type.BaseType != typeof(Entity))
            {
                type = type.BaseType;
            }
            return type;
        }

        protected object CorrectSqlDateTime(object parameterValue)
        {
            if (parameterValue != null && parameterValue.GetType().Name == "DateTime")
            {
                if (Convert.ToDateTime(parameterValue) < SqlDateTime.MinValue.Value)
                    return SqlDateTime.MinValue.Value;
                else
                    return parameterValue;
            }
            else
                return parameterValue;
        }
    }
}
