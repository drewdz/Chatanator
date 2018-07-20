using System.Collections.Generic;

namespace PE.Framework.DataProvider
{
    public interface IDataProvider
    {
        #region Operations

        /// <summary>
        /// Add a new item to the database
        /// </summary>
        /// <typeparam name="TItem">The type of item to add</typeparam>
        /// <param name="item">The item to add</param>
        void Add<TItem>(TItem item);

        /// <summary>
        /// Add new items to the database
        /// </summary>
        /// <typeparam name="TItem">The type of the items to add</typeparam>
        /// <param name="items">The items to add</param>
        void AddAll<TItem>(IEnumerable<TItem> items);

        /// <summary>
        /// Remove an item from the database
        /// </summary>
        /// <typeparam name="TItem">The type of the item to remove</typeparam>
        /// <param name="item">The item to remove</param>
        /// <remarks>It should be possible to pass in an item with only a primary key to remove</remarks>
        void Delete<TItem>(TItem item);

        /// <summary>
        /// Remove multiple items from the database
        /// </summary>
        /// <typeparam name="TItem">The type of the items to remove</typeparam>
        /// <param name="items">The items to remove</param>
        /// <remarks>It should be possible to pass in an item with only a primary key to remove</remarks>
        void DeleteAll<TItem>(IEnumerable<TItem> items);

        /// <summary>
        /// Update an item in the database. This item will insert a new instance of the item if it doesn't have a [PrimaryKey] property
        /// </summary>
        /// <typeparam name="TItem">The type of item to update</typeparam>
        /// <param name="item">The item to update</param>
        void Update<TItem>(TItem item);

        #endregion Operations
    }
}
