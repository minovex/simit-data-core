namespace Simit.Data.Core.Database
{
	#region Using Directives
	using System;
	using System.Collections.Generic;
    using System.Data.Common;
	using System.Data;
    using System.Configuration;
	#endregion

    public sealed class DatabaseProvider 
    {  
        #region Private Fields
        /// <summary>
        /// The provider
        /// </summary>
        private DbProviderFactory provider;
        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProvider"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="System.ArgumentNullException">connectionStringName</exception>
        /// <exception cref="Simit.Data.Core.ConnectionStringNotFoundException"></exception>
        public DatabaseProvider(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName)) throw new ArgumentNullException("connectionStringName");
            if (ConfigurationManager.ConnectionStrings[connectionStringName] == null) throw new ConnectionStringNotFoundException();

            this.provider = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);
            this.connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            return this.provider.CreateConnection();
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public IDbCommand CreateCommand(int? timeout)
        {
            IDbCommand command =  this.provider.CreateCommand();
            
            if (timeout.HasValue)
                command.CommandTimeout = timeout.Value;
            return command;
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter CreateParameter()
        {
            return this.provider.CreateParameter();
        }

        /// <summary>
        /// Creates the open connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateOpenConnection()
        {
            IDbConnection connection =  this.CreateConnection();
            connection.ConnectionString = this.connectionString;
            connection.Open();

            return connection;
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// commandText
        /// or
        /// connection
        /// </exception>
        public IDbCommand CreateCommand(string commandText, IDbConnection connection, int? timeout)
        {
            if (String.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            if (connection == null)
                throw new ArgumentNullException("connection");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = commandText;
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            return command;
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameterName
        /// or
        /// parameterValue
        /// </exception>
        public IDataParameter CreateParameter(string parameterName, object parameterValue)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            if (parameterValue == null)
                throw new ArgumentNullException("parameterValue");

            IDataParameter parameter = this.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            return parameter;
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            return this.CreateOpenConnection().BeginTransaction();
        }
        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            return this.CreateOpenConnection().BeginTransaction(level);
        }

        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// transaction
        /// or
        /// parameters
        /// </exception>
        public IDbCommand CreateStoredProcedureCommand(string procedureName, List<IDbDataParameter> parameters, IDbTransaction transaction, int? timeout)
        {
            if (String.IsNullOrEmpty(procedureName)) throw new ArgumentNullException("procedureName");
            if (transaction == null) throw new ArgumentNullException("transaction");

            IDbCommand command = this.CreateCommand(timeout);
            command.CommandText = procedureName;
            command.Connection = transaction.Connection;
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = transaction;

            if (parameters != null)
            {
                foreach (IDbDataParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// connection
        /// </exception>
        public IDbCommand CreateStoredProcedureCommand(string procedureName, List<IDbDataParameter> parameters, IDbConnection connection, int? timeout)
        {
            if (String.IsNullOrEmpty(procedureName)) throw new ArgumentNullException("procedureName");
            if (connection == null) throw new ArgumentNullException("connection");

            IDbCommand command = this.CreateCommand(timeout);
            command.CommandText = procedureName;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
          
            if (parameters != null)
            {
                foreach (IDbDataParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// transaction
        /// </exception>
        public IDbCommand CreateStoredProcedureCommand(string procedureName, IDbTransaction transaction, int? timeout)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = procedureName;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.Connection = transaction.Connection;
            command.CommandType = CommandType.StoredProcedure;
          

            return command;
        }

        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// connection
        /// </exception>
        public IDbCommand CreateStoredProcedureCommand(string procedureName, IDbConnection connection, int? timeout)
        {

            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            if (connection == null)
                throw new ArgumentNullException("connection");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = procedureName;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;

            return command;
        }

        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procedureName
        /// or
        /// connection
        /// or
        /// parameters
        /// </exception>
        public IDbCommand CreateStoredProcedureCommand(string procedureName, IDbConnection connection, List<IDbDataParameter> parameters, int? timeout)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            if (connection == null)
                throw new ArgumentNullException("connection");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = procedureName;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;

            foreach (IDbDataParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Creates the text command.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// transaction
        /// or
        /// parameters
        /// </exception>
        public IDbCommand CreateTextCommand(string text, List<IDbDataParameter> parameters, IDbTransaction transaction, int? timeout)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = text;
            command.Transaction = transaction;
            command.Connection = transaction.Connection;
            command.CommandType = CommandType.Text;
          

            foreach (IDbDataParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Creates the text command.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// transaction
        /// </exception>
        public IDbCommand CreateTextCommand(string text, IDbTransaction transaction, int? timeout)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = text;
            command.Transaction = transaction;
            command.Connection = transaction.Connection;
            command.CommandType = CommandType.Text;

            return command;
        }

        /// <summary>
        /// Creates the text command.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// connection
        /// </exception>
        public IDbCommand CreateTextCommand(string text, IDbConnection connection, int? timeout)
        {

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (connection == null)
                throw new ArgumentNullException("connection");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = text;
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            if (timeout.HasValue)
                command.CommandTimeout = timeout.Value;

            return command;
        }

        /// <summary>
        /// Creates the text command.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// connection
        /// or
        /// parameters
        /// </exception>
        public IDbCommand CreateTextCommand(string text, IDbConnection connection, List<IDbDataParameter> parameters, int? timeout)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (connection == null)
                throw new ArgumentNullException("connection");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            IDbCommand command = this.CreateCommand(timeout);

            command.CommandText = text;
            command.Connection = connection;
            command.CommandType = CommandType.Text;
          

            foreach (IDbDataParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
        #endregion

      
    }
}