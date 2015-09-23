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
    public class PropertyNotFoundExpception : Exception
    {
        #region Public Constructors
        public PropertyNotFoundExpception()
            : base()
        {

        }
        public PropertyNotFoundExpception(string typeName, string propertyName)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} not found in {1}.", propertyName, typeName))
        {

        }
        public PropertyNotFoundExpception(string typeName, string propertyName, Exception exception)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} not found {1}.", propertyName, typeName), exception)
        {

        }
        #endregion

        #region Protected Constructors
        protected PropertyNotFoundExpception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}