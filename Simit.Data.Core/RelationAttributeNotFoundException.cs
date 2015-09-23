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
    public class RelationAttributeNotFoundException : Exception
    {
        #region Public Constructors
        public RelationAttributeNotFoundException()
            : base()
        {

        }
        public RelationAttributeNotFoundException(string parameterName)
            : base(String.Format(CultureInfo.CurrentCulture, "[RelationAttribute] not found in {0} parameter.", parameterName))
        {

        }
        public RelationAttributeNotFoundException(string parameterName, Exception exception)
            : base(String.Format(CultureInfo.CurrentCulture, "[RelationAttribute] not found in {0} parameter.", parameterName), exception)
        {

        }
        #endregion

        #region Protected Constructors
        protected RelationAttributeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}