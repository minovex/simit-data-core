using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simit.Data.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelationAttribute : Attribute
    {
        private string typeName;
        private string methodName;
        private string propertyNames;

        public string TypeName
        {
            get { return this.typeName; }
            set { this.typeName = value; }
        }
        public string PropertyNames
        {
            get { return this.propertyNames; }
            set { this.propertyNames = value; }
        }
        public string MethodName
        {
            get { return this.methodName; }
            set { this.methodName = value; }
        }

        public RelationAttribute()
        { 
        
        }

        public RelationAttribute(string TypeName, string MethodName, string PropertyNames)
        {
            this.typeName = TypeName;
            this.propertyNames = PropertyNames;
            this.methodName = MethodName;
        }
    }
}