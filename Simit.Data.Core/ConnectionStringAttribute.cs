namespace Simit.Data.Core
{
    #region Using Directives
    using System;
    #endregion

    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringAttribute : Attribute
    {
        #region Public Properties
        public string Name { get; set; }
        #endregion
    }
}