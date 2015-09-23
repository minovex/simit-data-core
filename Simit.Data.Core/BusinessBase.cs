#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using System.Collections.ObjectModel;
#endregion

namespace Simit.Data.Core
{
    public abstract class BusinessBase : Simit.Data.Core.Database.DatabaseAdapter
    {
        #region Protected Constructors
        protected BusinessBase()
        {

        }
        #endregion

        #region Protected Methods
        protected T Map<T>(DataRow row) where T : class
        {
            Type type = typeof(T);
            object t = Activator.CreateInstance(type);
            PropertyInfo property;
            foreach (DataColumn column in row.Table.Columns)
            { 
                if (row[column.ColumnName] != DBNull.Value)
                {
                    property = t.GetType().GetProperty(column.ColumnName);
                    if (property != null)
                        property.SetValue(t, row[column.ColumnName], null);
                }
            }

            return (T)t;
        }
        protected List<T> MapAll<T>(DataTable table) where T : class
        {
            List<T> list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(this.Map<T>(row));
            }

            table = null;

            return list;
        }

        protected List<T> ExecutePagedStoredProcedure<T>(string storedProcedure, string countTableColumnName, List<IDbDataParameter> parameterList, int numberOnPage, out int totalRowCount, out int totalPage) where T : class
        {
            if (string.IsNullOrEmpty(storedProcedure)) throw new ArgumentNullException("storedProcedure");
            if (string.IsNullOrEmpty(countTableColumnName)) throw new ArgumentNullException("countTableColumnName");
            if (numberOnPage <= 0) throw new ArgumentException("numberOnPage must be greater than zero");

            totalRowCount = 0;
            totalPage = 0;

            DataTable table = base.ExecuteStoredProcedureToDataTable(storedProcedure, parameterList);

            if (table != null && table.Rows.Count > 0 && table.Columns.Contains(countTableColumnName))
            {
                totalRowCount = (int)table.Rows[0][countTableColumnName];

                totalPage = (int)Math.Ceiling((double)totalRowCount / (double)numberOnPage);

                return this.MapAll<T>(table);
            }

            return new List<T>();
        }
        #endregion
    }
}