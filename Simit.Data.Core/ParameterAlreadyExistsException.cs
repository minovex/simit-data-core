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
    public class ParameterAlreadyExistsException : Exception
    {
        #region Public Constructors
        public ParameterAlreadyExistsException()
            : base()
        { 
        
        }
        public ParameterAlreadyExistsException(string parameterName)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} parameter already exists.", parameterName))
        { 
        
        }
        public ParameterAlreadyExistsException(string parameterName, Exception exception)
            : base(String.Format(CultureInfo.CurrentCulture, "{0} parameter already exists.", parameterName), exception)
        {

        }
        #endregion

        #region Protected Constructors
        protected ParameterAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}