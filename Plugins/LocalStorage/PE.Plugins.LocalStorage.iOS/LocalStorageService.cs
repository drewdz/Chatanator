using PE.Framework.Serialization;

using System;
using System.Collections.Generic;
using System.IO;

namespace PE.Plugins.LocalStorage.iOS
{
    public class LocalStorageService : ILocalStorageService
    {
        #region Constants

        private const string SETTINGS = "Settings";

        #endregion Constants

        #region Fields

        private Dictionary<string, string> _Settings;

        #endregion Fields

        #region Constructors

        public LocalStorageService()
        {
            Initialize();
        }

        #endregion Constructors

        #region Put

        public void Put<TEntity>(TEntity entity)
        {
            Put(typeof(TEntity).Name, entity);
        }

        public void Put<TEntity>(string name, TEntity entity)
        {
            name = GetPath(name);
            var data = System.Text.Encoding.UTF8.GetBytes(Serializer.Serialize(entity));

            if (File.Exists(name))
            {
                File.Delete(name);
            }

            using (var stream = File.Open(name, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        #endregion Put

        #region Get

        public TEntity Get<TEntity>()
        {
            return Get<TEntity>(typeof(TEntity).Name);
        }

        public TEntity Get<TEntity>(string name)
        {
            try
            {
                //  open the file
                name = GetPath(name);
                if (!File.Exists(name)) return default(TEntity);

                using (Stream stream = File.Open(name, FileMode.Open, FileAccess.Read))
                {
                    using (var ms = new MemoryStream())
                    {
                        while (true)
                        {
                            byte[] d = new byte[1024];
                            int result = stream.Read(d, 0, 1024);
                            ms.Write(d, 0, result);
                            if (result < 1024) break;
                        }
                        //  deserialize
                        ms.Seek(0, SeekOrigin.Begin);
                        byte[] data = ms.ToArray();
                        return Serializer.Deserialize<TEntity>(System.Text.Encoding.UTF8.GetString(data, 0, data.Length));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return default(TEntity);
            }
        }

        #endregion Get

        #region Delete

        public void Delete(string name)
        {
            try
            {
                name = GetPath(name);
                if (File.Exists(name)) File.Delete(name);
            }
            catch { /* do nothing */ }
        }

        #endregion Delete

        #region Exists

        public bool Exists(string key)
        {
            try
            {
                //  open the file
                key = GetPath(key);
                return (!File.Exists(key)) ? false : true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        #endregion Exists

        #region Operations

        public void Write(string key, string value)
        {
            if (_Settings.ContainsKey(key))
                _Settings[key] = value;
            else
                _Settings.Add(key, value);
        }

        public string Read(string key)
        {
            //  check if the setting exists
            return (_Settings.ContainsKey(key)) ? _Settings[key] : string.Empty;
        }

        #endregion Operations

        #region Private Methods

        private string GetPath(string path)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var library = Path.Combine(documents, "..", "Library");
            return Path.Combine(library, path + ".json");
        }

        private void Initialize()
        {
            _Settings = Get<Dictionary<string, string>>(SETTINGS);
            if (_Settings == null) _Settings = new Dictionary<string, string>();
        }

        #endregion Private Methods
    }
}