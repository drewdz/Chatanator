using PE.Framework.Serialization;

using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;

namespace PE.Plugins.LocalStorage.WindowsCommon
{
    public class LocalStorageService : ILocalStorageService
    {
        #region Fields

        private readonly StorageFolder _LocalStorage;
        private readonly ApplicationDataContainer _Settings;

        #endregion Fields

        #region Constructors

        public LocalStorageService()
        {
            _Settings = ApplicationData.Current.LocalSettings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always);
            _LocalStorage = ApplicationData.Current.LocalFolder;
        }

        #endregion Constructors

        #region Put

        private async Task PutAsync<TEntity>(string name, TEntity entity)
        {
            try
            {
                var file = await _LocalStorage.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    Serializer.Serialize(entity, stream);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("*** LocalStorageManager.PutAsync - Exception: {0}", ex);
            }
        }

        public void Put<TEntity>(TEntity entity)
        {
            Put(typeof(TEntity).Name, entity);
        }

        public void Put<TEntity>(string name, TEntity entity)
        {
            Task.Run(async () => await PutAsync(name, entity));
        }

        #endregion Put

        #region Get

        public TEntity Get<TEntity>()
        {
            return Task.Run(async () => await GetAsync<TEntity>(typeof(TEntity).Name)).Result;
        }

        public TEntity Get<TEntity>(string name)
        {
            return Task.Run(async () => await GetAsync<TEntity>(name)).Result;
        }

        private async Task<TEntity> GetAsync<TEntity>(string name)
        {
            try
            {
                //  open the file
                var file = await _LocalStorage.GetFileAsync(name);
                if (file == null) return default(TEntity);

                //  get the data
                using (var stream = await file.OpenStreamForReadAsync())
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
            catch (FileNotFoundException fex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** LocalStorageManager.GetAsync - File Not Found: {0}", name));
                return default(TEntity);
            }
        }

        #endregion Get

        #region Delete

        public void Delete(string name)
        {
            Task.Run(async () => await DeleteAsync(name));
        }

        private async Task DeleteAsync(string name)
        {
            try
            {
                //  open the file
                var file = await _LocalStorage.GetFileAsync(name);
                if (file == null) return;
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("LocalStorageService.DeleteAsync - Exception: {0}", ex));
            }
        }

        #endregion Delete

        #region Exists

        public bool Exists(string key)
        {
            return Task.Run(async () => await ExistsAsync(key)).Result;
        }

        private async Task<bool> ExistsAsync(string key)
        {
            try
            {
                //  open the file
                var file = await _LocalStorage.GetFileAsync(key);
                return (file == null) ? false : true;
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
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public string Read(string key)
        {
            try
            {
                return (string)ApplicationData.Current.LocalSettings.Values[key];
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion Operations
    }
}
