#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;
#endregion

namespace Simit.Data.Core
{
    public class DataLoadOptionItem
    {
        #region Private Fields
        private SortedList<int, string> innerProperties = new SortedList<int, string>();
        private int indexer = 1;
        private string propertyPath;       
        #endregion

        #region Public Fields
        public static readonly string Suffix = ".";
        #endregion

        #region Internal Properties
        internal SortedList<int, string> InnerProperties
        {
            get { return this.innerProperties; }
            set { this.innerProperties = value; }
        }
        internal string PropertyPath
        {
            get
            {
                if (String.IsNullOrEmpty(this.propertyPath))
                    this.propertyPath = this.CreatePropertyPath();

                return this.propertyPath;
            }
            set
            {
                this.propertyPath = value;
            }
        }
        internal int Depth
        {
            get { return this.PropertyPath.Split('.').Length; }
        }
        #endregion

        #region Public Methods
        public DataLoadOptionItem AddInnerProperty<T>(Expression<Func<T, object>> expression)
        {
            MemberExpression parameter = expression.Body as MemberExpression;
            if (parameter == null)
                parameter = expression.Body is UnaryExpression ? ((UnaryExpression)expression.Body).Operand as MemberExpression : null;

            if (parameter == null)
            {          
                throw new InvalidOperationException("Only property required.");
            }
            if (parameter.Member.GetCustomAttributes(typeof(RelationAttribute), false).Length == 0)
            {
                throw new RelationAttributeNotFoundException(parameter.Member.Name);
            }

            this.AddProperty(typeof(T).FullName, parameter.Member.Name);

            return this;
        }
        #endregion

        #region Private Methods
        private string CreatePropertyPath()
        {
            string[] propertyList = new string[this.innerProperties.Count];
            int indexer = 0;
            foreach (int order in this.innerProperties.Keys)
            {
                propertyList[indexer++] = this.innerProperties[order];
            }
            return String.Join(".", propertyList);
        }
        private void AddProperty(string typeFullName, string propertyName)
        {
            if (String.IsNullOrEmpty(typeFullName))
                throw new ArgumentNullException("typeFullName");

            if (String.IsNullOrEmpty(typeFullName))
                throw new ArgumentNullException("propertyName");

            innerProperties.Add(this.indexer, typeFullName);
            this.indexer++;
            innerProperties.Add(this.indexer, propertyName);
            this.indexer++;
        }
        #endregion

        #region Internal Methods
        internal bool IsProperty(string parentObjectName)
        {
            if (String.IsNullOrEmpty(parentObjectName))
                throw new ArgumentNullException("parentObjectName");

            return this.PropertyPath.StartsWith(parentObjectName + DataLoadOptionItem.Suffix) && this.PropertyPath.Substring(parentObjectName.Length + DataLoadOptionItem.Suffix.Length).Split('.').Length == 1;            
        }
        internal string GetProperty(string parentObjectName)
        {
            if (String.IsNullOrEmpty(parentObjectName))
                throw new ArgumentNullException("parentObjectName");

            return this.PropertyPath.Substring(parentObjectName.Length + Suffix.Length);
        }      
        #endregion
    }
}