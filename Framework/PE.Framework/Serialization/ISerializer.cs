namespace PE.Framework.Serialization
{
    public interface ISerializer
    {
        string Serialize<TEntity>(TEntity entity);

        TEntity Deserialize<TEntity>(string data);
    }
}
