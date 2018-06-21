namespace PE.Plugins.LocalStorage
{
    public interface ILocalStorageService
    {
        void Put<TEntity>(TEntity entity);

        void Put<TEntity>(string name, TEntity entity);

        TEntity Get<TEntity>();

        TEntity Get<TEntity>(string name);

        void Delete(string name);

        void Write(string key, string value);

        bool Exists(string key);

        string Read(string key);
    }
}
