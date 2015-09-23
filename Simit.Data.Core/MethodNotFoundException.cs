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
    public class MethodNotFoundException : Exception
    {
        #region Public Constructors
        public MethodNotFoundException()
            : base()
        {

        }
        public MethodNotFoundException(string typeName, string methodName)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} not found in {1}.", methodName, typeName))
        {

        }
        public MethodNotFoundException(string typeName, string methodName, Exception exception)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} not found {1}.", methodName, typeName), exception)
        {

        }
        #endregion

        #region Protected Constructors
        protected MethodNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}