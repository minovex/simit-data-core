namespace Simit.Data.Core
{
    #region Using Directives
    using System;
    using System.Data;
    #endregion

    public sealed class DataRowAdapter : IIndexedDataRecord
    {
        #region Private Fields
        /// <summary>
        /// The row
        /// </summary>
        private readonly DataRow row;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowAdapter"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <exception cref="System.ArgumentNullException">row</exception>
        public DataRowAdapter(DataRow row)
        {
            if (row == null) throw new ArgumentNullException("row");

            this.row = row;
        }
        #endregion

        #region Interface Implementations
        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object this[string name]
        {
            get { return this.row[name]; }
        }
        #endregion
    }
}
