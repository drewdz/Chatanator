using PE.Framework.DataProvider;

using Realms;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PE.Provider.Data.Realm
{
    public class DataProvider : IDataProvider
    {
        #region Fields

        protected RealmConfiguration _Configuration;

        #endregion Fields

        #region Constructors

        public DataProvider(object configuration)
        {
            if ((configuration != null) && (configuration is RealmConfiguration))
            {
                _Configuration = (RealmConfiguration)configuration;
                _Configuration.MigrationCallback = Migration;
            }
        }

        #endregion Constructors

        #region Properties

        public virtual Realms.Realm Database
        {
            get
            {
                try
                {
                    return Realms.Realm.GetInstance(_Configuration ?? null);
                }
                catch (Exception ex)
                {
                    Realms.Realm.DeleteRealm(_Configuration ?? null);
                    return Realms.Realm.GetInstance(_Configuration ?? null);
                }
            }
        }

        #endregion Properties

        #region Add

        /// <summary>
        /// Add a new item to the database
        /// </summary>
        /// <typeparam name="TItem">The type of item to add</typeparam>
        /// <param name="item">The item to add</param>
        public virtual void Add<TItem>(TItem item)
        {
            if (!(item is RealmObject)) throw new System.ArgumentException("Can only write an object to the database that inherits from RealmObject");
            var realmItem = item as RealmObject;
            Database.Write(() =>
            {
                Database.Add(realmItem, true);
            });
        }

        /// <summary>
        /// Add new items to the database
        /// </summary>
        /// <typeparam name="TItem">The type of the items to add</typeparam>
        /// <param name="items">The items to add</param>
        public virtual void AddAll<TItem>(IEnumerable<TItem> items)
        {
            Database.Write(() =>
            {
                foreach (var item in items)
                {
                    if (!(item is RealmObject)) throw new System.ArgumentException("Can only write an object to the database that inherits from RealmObject");
                    var realmItem = item as RealmObject;
                    Database.Add(realmItem, true);
                }
            });
        }

        #endregion Add

        #region Delete

        /// <summary>
        /// Remove an item from the database
        /// </summary>
        /// <typeparam name="TItem">The type of the item to remove</typeparam>
        /// <param name="item">The item to remove</param>
        /// <remarks>It should be possible to pass in an item with only a primary key to remove</remarks>
        public virtual void Delete<TItem>(TItem item)
        {
            if (!(item is RealmObject)) throw new System.ArgumentException("Can only write an object to the database that inherits from RealmObject");
            var realmItem = item as RealmObject;
            Database.Write(() =>
            {
                Database.Remove(realmItem);
            });
        }

        /// <summary>
        /// Remove multiple items from the database
        /// </summary>
        /// <typeparam name="TItem">The type of the items to remove</typeparam>
        /// <param name="items">The items to remove</param>
        /// <remarks>It should be possible to pass in an item with only a primary key to remove</remarks>
        public virtual void DeleteAll<TItem>(IEnumerable<TItem> items)
        {
            Database.Write(() =>
            {
                foreach (var item in items)
                {
                    if (!(item is RealmObject)) throw new System.ArgumentException("Can only write an object to the database that inherits from RealmObject");
                    var realmItem = item as RealmObject;
                    Database.Remove(realmItem);
                }
            });
        }

        #endregion Delete

        #region Update

        /// <summary>
        /// Update an item in the database. This item will insert a new instance of the item if it doesn't have a [PrimaryKey] property
        /// </summary>
        /// <typeparam name="TItem">The type of item to update</typeparam>
        /// <param name="item">The item to update</param>
        public virtual void Update<TItem>(TItem item)
        {
            if (!(item is RealmObject)) throw new System.ArgumentException("Can only write an object to the database that inherits from RealmObject");
            var realmItem = item as RealmObject;
            Database.Write(() =>
            {
                Database.Add(realmItem, true);
            });
        }

        #endregion Update

        #region Migration

        protected virtual void Migration(Migration migration, ulong oldVersion)
        {
        }

        #endregion Migration
    }
}
