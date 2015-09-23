#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
#endregion

namespace Simit.Data.Core
{
    [Serializable]
    public class TypeNotFoundException : Exception
    {
        #region Public Constructors
        public TypeNotFoundException()
            : base()
        {

        }
        public TypeNotFoundException(string typeName)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} not found.", typeName))
        {

        }
        public TypeNotFoundException(string typeName, Exception exception)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} not found.", typeName), exception)
        {

        }
        #endregion

        #region Protected Constructors
        protected TypeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}