namespace Simit.Data.Core
{
    #region Using Directives
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Xml.Linq;
    using System.Linq;
    #endregion

    public static class Extensions
    {
        public static SqlXml ToSqlXml<T>(this List<T> list, string rootNodeName, string itemNodeName) where T : struct
        {
            XElement element = new XElement(rootNodeName, list.Select(c => new XElement(itemNodeName, c.ToString())).ToArray());

            return new SqlXml(element.CreateReader());
        }

    }
}
