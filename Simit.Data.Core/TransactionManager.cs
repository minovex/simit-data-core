namespace Simit.Data.Core
{
    #region Using Directives
    using System;
    using System.Data;
    using Simit.Data.Core.Database;
    using System.Linq;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public sealed class TransactionManager
    {
        #region Private Fields
        /// <summary>
        /// The database provider
        /// </summary>
        private DatabaseProvider databaseProvider;
        /// <summary>
        /// The transaction
        /// </summary>
        private IDbTransaction transaction;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>
        /// The transaction.
        /// </value>
        public IDbTransaction Transaction
        {
            get { return this.transaction; }
        }
        #endregion

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionManager"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="System.ArgumentNullException">connectionStringName</exception>
        public TransactionManager(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName)) throw new ArgumentNullException("connectionStringName");

            this.databaseProvider = new DatabaseProvider(connectionStringName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionManager"/> class.
        /// </summary>
        /// <param name="typeOfFactory">The type of factory.</param>
        /// <exception cref="System.ArgumentNullException">typeOfFactory</exception>
        public TransactionManager(Type typeOfFactory)
        {
            if (typeOfFactory == null) throw new ArgumentNullException("typeOfFactory");

            ConnectionStringAttribute attiribute = typeOfFactory.GetCustomAttributes(typeof(ConnectionStringAttribute), false).Cast<ConnectionStringAttribute>().FirstOrDefault();

            this.databaseProvider = new DatabaseProvider(attiribute.Name);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this.transaction = this.databaseProvider.BeginTransaction();
        }
        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="level">The level.</param>
        public void BeginTransaction(IsolationLevel level)
        {
            this.transaction = this.databaseProvider.BeginTransaction(level);
        }
        /// <summary>
        /// Commits this instance.
        /// </summary>
        public void Commit()
        {
            if (this.transaction != null)
                this.transaction.Commit();
              
        }
        /// <summary>
        /// Rolls the back.
        /// </summary>
        public void RollBack()
        {
            if (this.transaction != null)
                this.transaction.Rollback();
        }
        #endregion
    }
}