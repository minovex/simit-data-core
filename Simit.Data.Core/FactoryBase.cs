namespace Simit.Data.Core
{
	#region Using Directives
	using System;
    using System.Data;
    using System.Reflection;
	using System.Linq.Expressions;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections.Generic;
	#endregion

    public abstract class FactoryBase<TEntity> : Simit.Data.Core.Database.DatabaseAdapter
    {
        #region Private Methods
        /// <summary>
        /// The active connection
        /// </summary>
        private IDbConnection activeConnection;

        /// <summary>
        /// Gets or sets the entity cache.
        /// </summary>
        /// <value>
        /// The entity cache.
        /// </value>
        private Dictionary<string, Dictionary<string, object>> entityCache { get; set; }

        private Dictionary<string, FactoryCacheItem> factoryCache { get; set; }
        #endregion

        #region Public Fields
        /// <summary>
        /// The load options
        /// </summary>
        private DataLoadOptions loadOptions;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the load options.
        /// </summary>
        /// <value>
        /// The load options.
        /// </value>
        public DataLoadOptions LoadOptions
        {
            get { return this.loadOptions; }
			internal set {this.loadOptions = value;}
        }
        #endregion

        #region Protected Construstors
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryBase{TEntity}"/> class.
        /// </summary>
        protected FactoryBase()
        {
			this.loadOptions = new DataLoadOptions();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected FactoryBase(IDbConnection connection) : this()
        {
            this.activeConnection = connection;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Maps the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        protected abstract TEntity Map(IIndexedDataRecord record);
		
        /// <summary>
        /// Maps all.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="MinikTaraftar.Data.Core.ArgumentNullException">reader</exception>
        protected List<TEntity> MapAll(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            List<TEntity> List = new List<TEntity>();
            TEntity item;
            while (reader.Read())
            {
                 item = Map(new DataReaderAdapter(reader));
                List.Add(item);
            }
            return List;
        }

        /// <summary>
        /// Maps all.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="MinikTaraftar.Data.Core.ArgumentNullException">procedureName</exception>
        protected List<TEntity> MapAll(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            List<TEntity> output = null;

            if (this.activeConnection != null)
            {
                output = this.MapAll(this.activeConnection, procedureName, parameters, timeout);
            }
            else
            {
                using (IDbConnection connection = base.DatabaseProvider.CreateOpenConnection())
                {
                    this.activeConnection = connection;
                    output = this.MapAll(connection, procedureName, parameters, timeout);
                    this.activeConnection = null;
                }
            }
            return output;
        }
		
		/// <summary>
		/// Executes the paged stored procedure.
		/// </summary>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <param name="countTableColumnName">Name of the count table column.</param>
		/// <param name="parameterList">The parameter list.</param>
		/// <param name="numberOnPage">The number on page.</param>
		/// <param name="totalRowCount">The total row count.</param>
		/// <param name="totalPage">The total page.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">
		/// storedProcedure
		/// or
		/// countTableColumnName
		/// </exception>
		/// <exception cref="System.ArgumentException">numberOnPage must be greater than zero</exception>
		protected List<TEntity> ExecutePagedStoredProcedure(string storedProcedure, string countTableColumnName, List<IDbDataParameter> parameterList, int numberOnPage, out int totalRowCount, out int totalPage)
		{
			if (string.IsNullOrEmpty(storedProcedure)) throw new ArgumentNullException("storedProcedure");
			if (string.IsNullOrEmpty(countTableColumnName)) throw new ArgumentNullException("countTableColumnName");
			if (numberOnPage <= 0) throw new ArgumentException("numberOnPage must be greater than zero");

			List<TEntity> list = new List<TEntity>();

			totalRowCount = 0;
			totalPage = 0;

			DataTable table = base.ExecuteStoredProcedureToDataTable(storedProcedure, parameterList);

			if (table != null && table.Rows.Count > 0 && table.Columns.Contains(countTableColumnName))
			{
				totalRowCount = (int) table.Rows[0][countTableColumnName];

				totalPage = (int) Math.Ceiling((double) totalRowCount / (double) numberOnPage);

				foreach (DataRow row in table.Rows)
				{
					list.Add(Map(new DataRowAdapter(row)));
				}
			}

			return list;
		}

        /// <summary>
        /// Maps the specified procedure name.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="MinikTaraftar.Data.Core.ArgumentNullException">procedureName</exception>
        protected TEntity Map(string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            if (String.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            TEntity output = default(TEntity);


            if (this.activeConnection != null)
            {
                output = this.Map(this.activeConnection, procedureName, parameters, timeout);
            }
            else
            {
                using (IDbConnection connection = base.DatabaseProvider.CreateOpenConnection())
                {
                    this.activeConnection = connection;
                    output = this.Map(connection, procedureName, parameters, timeout);
                    this.activeConnection = null;
                }
            }

            
            return output;
        }

        /// <summary>
        /// Loads the properties.
        /// </summary>
        /// <param name="entitiy">The entitiy.</param>
        /// <exception cref="MinikTaraftar.Data.Core.MethodNotFoundException"></exception>
        /// <exception cref="MinikTaraftar.Data.Core.RelationAttributeNotFoundException"></exception>
        protected void LoadProperties(object entitiy)
        {
            if (entitiy == null || this.loadOptions == null) return;
            Type entityType = entitiy.GetType();
            string lastPropertyName;
            foreach (DataLoadOptionItem optionItem in loadOptions.Items)
            {
                if (optionItem.IsProperty(entityType.FullName))
                {
                    lastPropertyName = optionItem.GetProperty(entityType.FullName);
                    PropertyInfo property = entityType.GetProperty(lastPropertyName);
                    
                    if (property == null)  throw new MethodNotFoundException(entityType.FullName, lastPropertyName);

                    object[] attributes = property.GetCustomAttributes(typeof(RelationAttribute), false);

                    if (attributes.Length == 0)  throw new RelationAttributeNotFoundException(property.Name);

                    DataLoadOptions newOptions = loadOptions.CreateLoadOptionFromNewRoute(optionItem.PropertyPath);

                    RelationAttribute attribute = (RelationAttribute)attributes[0];

                    property.SetValue(entitiy, this.CreateObject(attribute, entitiy, newOptions, System.Reflection.Assembly.GetCallingAssembly(), this.activeConnection), null);

                }
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Only property required.</exception>
        protected string GetPropertyName(Expression<Func<TEntity, object>> expression)
        {
            MemberExpression parameter = expression.Body as MemberExpression;
            if (parameter == null)
                parameter = expression.Body is UnaryExpression ? ((UnaryExpression)expression.Body).Operand as MemberExpression : null;

            if (parameter == null)
            {
                throw new InvalidOperationException("Only property required.");
            }
            return parameter.Member.Name;
        }
		
		        /// <summary>
        /// Creates the batch.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TID">The type of the identifier.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="items">The items.</param>
        /// <param name="transaction">The transaction.</param>
        protected void CreateBatch<TItem, TID>(IRepository<TItem, TID> repository, List<TItem> items, IDbTransaction transaction = null)
			where TItem : class
			where TID : struct
        {
            if (transaction == null)
            {
                TransactionManager transctionManager = new TransactionManager(repository.GetType());
                transctionManager.BeginTransaction();

                try
                {
                    foreach (TItem entity in items)
                        repository.Create(entity, transctionManager.Transaction);

                    transctionManager.Commit();
                }
                catch
                {
                    transctionManager.RollBack();
                    throw;
                }
            }
            else
            {
                foreach (TItem entity in items)
                    repository.Create(entity, transaction);
            }
        }

        /// <summary>
        /// Updates the batch.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TID">The type of the identifier.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="items">The items.</param>
        /// <param name="transaction">The transaction.</param>
        protected void UpdateBatch<TItem, TID>(IRepository<TItem, TID> repository, List<TItem> items, IDbTransaction transaction = null)
			where TItem : class
			where TID : struct
        {
            if (transaction == null)
            {
                TransactionManager transctionManager = new TransactionManager(repository.GetType());
                transctionManager.BeginTransaction();

                try
                {
                    foreach (TItem entity in items)
                        repository.Update(entity, transctionManager.Transaction);

                    transctionManager.Commit();
                }
                catch
                {
                    transctionManager.RollBack();
                    throw;
                }
            }
            else
            {
                foreach (TItem entity in items)
                    repository.Create(entity, transaction);
            }
        }

        /// <summary>
        /// Deletes the batch.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TID">The type of the identifier.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="idList">The identifier list.</param>
        /// <param name="transaction">The transaction.</param>
        protected void DeleteBatch<TItem, TID>(IRepository<TItem, TID> repository, List<TID> idList, IDbTransaction transaction = null)
			where TItem : class
			where TID : struct
        {
            if (transaction == null)
            {
                TransactionManager transctionManager = new TransactionManager(repository.GetType());
                transctionManager.BeginTransaction();

                try
                {
                    foreach (TID ID in idList)
                        repository.Delete(ID, transctionManager.Transaction);

                    transctionManager.Commit();
                }
                catch
                {
                    transctionManager.RollBack();
                    throw;
                }
            }
            else
            {
                foreach (TID ID in idList)
                    repository.Delete(ID, transaction);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Clones the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">The type must be serializable.;source</exception>
		private T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="activeConnection">The active connection.</param>
        /// <returns></returns>
        /// <exception cref="MinikTaraftar.Data.Core.ArgumentNullException">
        /// attribute
        /// or
        /// entity
        /// </exception>
        /// <exception cref="MinikTaraftar.Data.Core.MethodNotFoundException">
        /// LoadOptions
        /// </exception>
        /// <exception cref="MinikTaraftar.Data.Core.TypeNotFoundException"></exception>
        private object CreateObject(RelationAttribute attribute, object entity, DataLoadOptions loadOptions, Assembly assembly, IDbConnection activeConnection)
        {
            System.Diagnostics.Debug.WriteLine(this.GetType().FullName + " -> CreateObject = " + attribute.TypeName);

            if (attribute == null) throw new ArgumentNullException("attribute");
            if (entity == null) throw new ArgumentNullException("entity");

            Type entityType = entity.GetType();

            string[] propertyNames = attribute.PropertyNames.Split(',');
            object[] parameters = new object[propertyNames.Length];

            PropertyInfo property;

            for (int i = 0; i < propertyNames.Length; i++)
            {
                property = entityType.GetProperty(propertyNames[i]);
               
                if (property == null) throw new MethodNotFoundException(entityType.FullName, propertyNames[i]);

                parameters[i] = property.GetValue(entity, null);
            }

            string itemHashCode = this.CreateItemHashCode(parameters);

            object cachedEntity = this.GetCachedEntity(itemHashCode, attribute.TypeName);

            if (cachedEntity == null)
            {

                if (factoryCache == null) factoryCache = new Dictionary<string, FactoryCacheItem>();

                if (!factoryCache.ContainsKey(attribute.TypeName))
                {
                    Type factoryType = assembly.GetType(attribute.TypeName);
                    var factoryObject = activeConnection != null ? Activator.CreateInstance(factoryType, new object[] { activeConnection }) : Activator.CreateInstance(assembly.GetType(attribute.TypeName));

                    this.factoryCache.Add(attribute.TypeName, new FactoryCacheItem {  Type = factoryType, Instance = factoryObject});

                }

                FactoryCacheItem factoryClass = this.factoryCache[attribute.TypeName];
                

                if (factoryClass == null) throw new TypeNotFoundException(attribute.TypeName);

                MethodInfo methodInfo = factoryClass.Type.GetMethod(attribute.MethodName);

                if (methodInfo == null) throw new MethodNotFoundException(attribute.TypeName, attribute.MethodName);

                property = factoryClass.Type.GetProperty("LoadOptions");

                if (property == null) throw new MethodNotFoundException(attribute.TypeName, "LoadOptions");


                property.SetValue(factoryClass.Instance, loadOptions, null);
                object value = methodInfo.Invoke(factoryClass.Instance, parameters);

                this.AddCache(itemHashCode, attribute.TypeName, value);

                return value;
            }
            else
            {
                return this.Clone(cachedEntity);
            }
        }

        /// <summary>
        /// Gets the cached entity.
        /// </summary>
        /// <param name="itemHashCode">The item hash code.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        private object GetCachedEntity(string itemHashCode, string typeName)
        {
            if (this.entityCache == null) this.entityCache = new Dictionary<string,Dictionary<string,object>>();
            if (!this.entityCache.ContainsKey(typeName)) this.entityCache.Add(typeName, new Dictionary<string,object>());

            if (!this.entityCache[typeName].ContainsKey(itemHashCode)) return null;

            object enttiy = this.entityCache[typeName][itemHashCode];

            if (enttiy != null)
                return enttiy;
            else
                return null;
        }

        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <param name="itemHashCode">The item hash code.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="value">The value.</param>
        private void AddCache(string itemHashCode, string typeName, object value)
        {
            if (!string.IsNullOrEmpty(itemHashCode))
                this.entityCache[typeName].Add(itemHashCode, value);
        }

        /// <summary>
        /// Creates the item hash code.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private string CreateItemHashCode(object[] parameters)
        {
            List<string> items = new List<string>();
            foreach (object item in parameters)
            {
                items.Add(item != null ? item.ToString() : string.Empty);
            }
            return string.Join(",", items.ToArray());

        }

        /// <summary>
        /// Maps all.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        private List<TEntity> MapAll(IDbConnection connection, string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            List<TEntity> output = null;
            using (IDbCommand command = parameters == null ? base.DatabaseProvider.CreateStoredProcedureCommand(procedureName, connection, timeout) : base.DatabaseProvider.CreateStoredProcedureCommand(procedureName, connection, parameters, timeout))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    output = MapAll(reader);
                }
            }

            if (this.entityCache != null)
                this.entityCache = null;

            return output;
        }

        /// <summary>
        /// Maps the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        private TEntity Map(IDbConnection connection, string procedureName, List<IDbDataParameter> parameters, int? timeout = null)
        {
            TEntity output = default(TEntity);
            
            using (IDbCommand command = parameters == null ? base.DatabaseProvider.CreateStoredProcedureCommand(procedureName, connection, timeout) : base.DatabaseProvider.CreateStoredProcedureCommand(procedureName, connection, parameters, timeout))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                         output = this.Map(new DataReaderAdapter(reader));
                    }
                }
            }
            
            return output;
        }
        #endregion
    }
}