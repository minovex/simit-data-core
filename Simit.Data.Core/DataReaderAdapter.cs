namespace Simit.Data.Core
{
    #region Using Directives
    using System;
    using System.Data;
    #endregion

    public sealed class DataReaderAdapter : IIndexedDataRecord
    {
        #region Private Fields
        /// <summary>
        /// The record
        /// </summary>
        private readonly IDataRecord record;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderAdapter"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="System.ArgumentNullException">reader</exception>
        public DataReaderAdapter(IDataRecord reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            this.record = reader;
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
            get { return this.record[name]; }
        }
        #endregion
    }
}
