namespace Simit.Data.Core.Database
{
	#region Using Directives
	using System;
	using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
	#endregion

    /// <summary>
    /// 
    /// </summary>
    public abstract class DatabaseAdapter
    {
        #region Private Fields
        /// <summary>
        /// The database provider
        /// </summary>
        private DatabaseProvider databaseProvider;
        #endregion

        #region Protected Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAdapter"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">ConnectionStringAttribute not defined in factory class</exception>
        protected DatabaseAdapter()
        {
            object[] connectionStringAttributes = this.GetType().GetCustomAttributes(typeof(ConnectionStringAttribute), false);
            if (connectionStringAttributes == null || connectionStringAttributes.Length == 0)
            {
                throw new InvalidOperationException("ConnectionStringAttribute not defined in factory class");
            }
            ConnectionStringAttribute attribute = connectionStringAttributes[0] as ConnectionStringAttribute;

            this.databaseProvider = new DatabaseProvider(attribute.Name);
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets or sets the database provider.
        /// </summary>
        /// <value>
        /// The database provider.
        /// </value>
        protected DatabaseProvider DatabaseProvider
        {
            get { return this.databaseProvider; }
            set { this.databaseProvider = value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// transaction
        /// </exception>
        protected int ExecuteStoredProcedure(string procedureName, List<IDbDataParameter> parameters, IDbTransaction transaction = null, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName)) throw new ArgumentNullException("procedureName");

            if (transaction != null)
            {
                using (IDbCommand command = databaseProvider.CreateStoredProcedureCommand(procedureName, parameters, transaction, timeout))
                {
                    return command.ExecuteNonQuery();
                }
            }
            else
            {
                using (IDbConnection connection = databaseProvider.CreateOpenConnection())
                {
                    using (IDbCommand command = databaseProvider.CreateStoredProcedureCommand(procedureName, parameters, connection, timeout))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
            }
         
        }
		
        /// <summary>
        /// Executes the command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        protected void ExecuteCommandText(string commandText, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateTextCommand(commandText, connection, timeout) : databaseProvider.CreateTextCommand(commandText, connection, parameters, timeout))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
		
        /// <summary>
        /// Executes the command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="System.ArgumentNullException">
        /// commandText
        /// or
        /// transaction
        /// </exception>
        protected void ExecuteCommandText(string commandText, List<IDbDataParameter> parameters, IDbTransaction transaction, int? timeout = null)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            if (transaction == null)
                throw new ArgumentNullException("transaction");


            using (IDbCommand command = parameters == null ? databaseProvider.CreateTextCommand(commandText, transaction, timeout) : databaseProvider.CreateTextCommand(commandText, parameters, transaction, timeout))
            {
                command.ExecuteNonQuery();
            }
        }
		
        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">procedureName</exception>
        protected T ExecuteStoredProcedureScalar<T>(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateStoredProcedureCommand(procedureName, connection, timeout) : databaseProvider.CreateStoredProcedureCommand(procedureName, connection, parameters, timeout))
                {
                   object value = command.ExecuteScalar();
                   if (value == DBNull.Value)
                       return default(T);
                   else
                       return (T)value;
                }
            }
        }
		
        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// transaction
        /// </exception>
        protected T ExecuteStoredProcedureScalar<T>(string procedureName, List<IDbDataParameter> parameters, IDbTransaction transaction, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            if (transaction == null)
                throw new ArgumentNullException("transaction");


            using (IDbCommand command = parameters == null ? databaseProvider.CreateStoredProcedureCommand(procedureName, transaction, timeout) : databaseProvider.CreateStoredProcedureCommand(procedureName, parameters, transaction, timeout))
            {
                object value = command.ExecuteScalar();
                if (value == DBNull.Value)
                    return default(T);
                else
                    return (T)value;
            }

        }
		
        /// <summary>
        /// Executes the command text scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        protected T ExecuteCommandTextScalar<T>(string commandText, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateTextCommand(commandText, connection, timeout) : databaseProvider.CreateTextCommand(commandText, connection, parameters, timeout))
                {
                    object value = command.ExecuteScalar();
                    if (value == DBNull.Value)
                        return default(T);
                    else
                        return (T)value;
                }
            }
        }
		
        /// <summary>
        /// Executes the command text scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// commandText
        /// or
        /// transaction
        /// </exception>
        protected T ExecuteCommandTextScalar<T>(string commandText, List<IDbDataParameter> parameters, IDbTransaction transaction, int? timeout = null)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            using (IDbCommand command = parameters == null ? databaseProvider.CreateTextCommand(commandText, transaction, timeout) : databaseProvider.CreateTextCommand(commandText, parameters, transaction, timeout))
            {
                object value = command.ExecuteScalar();
                if (value == DBNull.Value)
                    return default(T);
                else
                    return (T)value;
            }

        }
		
        /// <summary>
        /// Executes the stored procedure to data table.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">procedureName</exception>
        protected DataTable ExecuteStoredProcedureToDataTable(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            DataTable table = null;

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateStoredProcedureCommand(procedureName, connection, timeout) : databaseProvider.CreateStoredProcedureCommand(procedureName, connection, parameters, timeout))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        table = new DataTable();
                        table.Locale = CultureInfo.InvariantCulture;
                        table.Load(reader);
                    }
                }
            }
            return table;

        }
		
        /// <summary>
        /// Executes the stored procedure to data tables.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">procedureName</exception>
        protected DataTable[] ExecuteStoredProcedureToDataTables(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            List<DataTable> list = null;

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateStoredProcedureCommand(procedureName, connection, timeout) : databaseProvider.CreateStoredProcedureCommand(procedureName, connection, parameters, timeout))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        list = new List<DataTable>();
                        while (!reader.IsClosed)
                        {
                            DataTable table = new DataTable();
                            table.Locale = CultureInfo.InvariantCulture;
                            table.Load(reader);
                            list.Add(table);
                        }
                    }
                }
            }
            return list != null ? list.ToArray() : null;
        }
		
        /// <summary>
        /// Executes the command text to data table.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        protected DataTable ExecuteCommandTextToDataTable(string commandText, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            DataTable table = null;

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateTextCommand(commandText, connection, timeout) : databaseProvider.CreateTextCommand(commandText, connection, parameters, timeout))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        table = new DataTable();
                        table.Locale = CultureInfo.InvariantCulture;
                        table.Load(reader);
                    }
                }
            }
            return table;

        }
		
        /// <summary>
        /// Executes the command text to data tables.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        protected DataTable[] ExecuteCommandTextToDataTables(string commandText, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            List<DataTable> list = null;

            using (IDbConnection connection = databaseProvider.CreateOpenConnection())
            {
                using (IDbCommand command = parameters == null ? databaseProvider.CreateTextCommand(commandText, connection, timeout) : databaseProvider.CreateTextCommand(commandText, connection, parameters, timeout))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        list = new List<DataTable>();
                        while (!reader.IsClosed)
                        {
                            DataTable table = new DataTable();
                            table.Locale = CultureInfo.InvariantCulture;
                            table.Load(reader);
                            list.Add(table);
                        }
                    }
                }
            }
            return list != null ? list.ToArray() : null;
        }
		
		/// <summary>
        /// Gets the single column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">procedureName</exception>
        protected List<T> GetSingleColumn<T>(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            List<T> output = new List<T>();
            DataTable table = this.ExecuteStoredProcedureToDataTable(procedureName, parameters, timeout);

            foreach (DataRow row in table.Rows)
            {
                output.Add((T)row[0]);
            }


            return output;
        }
		
		/// <summary>
        /// Gets the double column.
        /// </summary>
        /// <typeparam name="T">type of key column</typeparam>
        /// <typeparam name="T2">type of value column</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">procedureName</exception>
        protected List<KeyValuePair<T, T2>> GetDoubleColumn<T, T2>(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            List<KeyValuePair<T, T2>> output = new List<KeyValuePair<T, T2>>();
            DataTable table = this.ExecuteStoredProcedureToDataTable(procedureName, parameters, timeout);

            foreach (DataRow row in table.Rows)
            {
                output.Add(new KeyValuePair<T, T2>((T)row[0], (T2)row[1]));
            }

            return output;
        }
        #endregion

    }
}