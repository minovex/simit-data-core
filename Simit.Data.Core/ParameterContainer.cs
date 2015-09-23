namespace Simit.Data.Core
{
    #region Using Directives
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data;
    #endregion

    public sealed class ParameterContainer
    {
        #region Private Fields
        private Database.DatabaseProvider databaseProvider;
        private Dictionary<string, IDbDataParameter> parameters;
        #endregion

        #region Public Constructors
        public ParameterContainer(Database.DatabaseProvider databaseProvider)
        {
            if (databaseProvider == null) throw new ArgumentNullException("databaseProvider");

            this.databaseProvider = databaseProvider;
            this.parameters = new Dictionary<string, IDbDataParameter>();
        }
        #endregion

        #region Public Indexers
        public IDbDataParameter this[string parameterName]
        {
            get
            {
                return this.parameters[parameterName];
            }
        }
        #endregion

        #region Public Methods
        public void AddInputParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (this.parameters.ContainsKey(name)) throw new ParameterAlreadyExistsException(name);

            IDbDataParameter parameter = this.databaseProvider.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.Direction = ParameterDirection.Input;

            this.parameters.Add(name, parameter);
        }
        public void AddOutPutParameter(string name, int size, DbType type)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (this.parameters.ContainsKey(name)) throw new ParameterAlreadyExistsException(name);

            IDbDataParameter parameter = this.databaseProvider.CreateParameter();
            parameter.ParameterName = name;
            parameter.Size = size;
            parameter.DbType = type;
            parameter.Direction = ParameterDirection.Output;

            this.parameters.Add(name, parameter);
        }
        public List<IDbDataParameter> ToList()
        {
            return this.parameters.Select(c => c.Value).ToList();
        }

        public ParameterContainer Input(string name, object value)
        {
            this.AddInputParameter(name, value);

            return this;
        }
        #endregion
    }
}
