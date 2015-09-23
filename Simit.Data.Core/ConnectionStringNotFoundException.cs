namespace Simit.Data.Core
{
    #region Using Directives
    using System;
    using System.Runtime.Serialization;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConnectionStringNotFoundException : Exception
    {
        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringNotFoundException"/> class.
        /// </summary>
        public ConnectionStringNotFoundException()
            : base("configuration/connectionStrings/add xml not found in application config file.")
        {

        }
        #endregion

        #region Protected Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ConnectionStringNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}