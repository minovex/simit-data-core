namespace Simit.Data.Core
{
	#region Using Directives
	using System;
	using System.Collections.Generic;
    using System.Linq.Expressions;
    #endregion

    public class DataLoadOptions
    {
        #region Private Fields
        /// <summary>
        /// The items
        /// </summary>
        private List<DataLoadOptionItem> items;
        #endregion
		
		#region Public Enums
        /// <summary>
        /// Sort Direction
        /// </summary>
        public enum SortDirection : int
        {
            Ascending = 1,
            Descending = 0
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<DataLoadOptionItem> Items
        {
            get { return this.items; }
        }
        #endregion

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadOptions"/> class.
        /// </summary>
        public DataLoadOptions()
        {
            this.items = new List<DataLoadOptionItem>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public DataLoadOptionItem AddProperty<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            DataLoadOptionItem item = new DataLoadOptionItem();
            item.AddInnerProperty<TEntity>(expression);
            items.Add(item);

            return item;
        }
        #endregion   
        
        #region Internal Methods
        /// <summary>
        /// Creates the load option from new route.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException>route</exception>
        internal DataLoadOptions CreateLoadOptionFromNewRoute(string route)
        {
            if (String.IsNullOrEmpty(route))
                throw new ArgumentNullException("route");

            DataLoadOptions option = new DataLoadOptions();

            foreach (DataLoadOptionItem item in this.items)
            {
                if (item.PropertyPath != route && item.PropertyPath.StartsWith(route))
                {
                    DataLoadOptionItem newItem = new DataLoadOptionItem();
                    newItem.InnerProperties = item.InnerProperties;
                    newItem.PropertyPath = item.PropertyPath.Substring(route.Length + DataLoadOptionItem.Suffix.Length);

                    option.AddItem(newItem);
                }
            }

            return option.Items.Count == 0 ? null : option;
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException>item</exception>
        internal void AddItem(DataLoadOptionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            items.Add(item);
        }
        #endregion
    }
}