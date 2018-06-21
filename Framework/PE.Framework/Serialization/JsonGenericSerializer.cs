namespace PE.Framework.Serialization
{
    public class JsonGenericSerializer : ISerializer
    {
        public string Serialize<TEntity>(TEntity entity)
        {
            return Serializer.Serialize<TEntity>(entity);
        }

        public TEntity Deserialize<TEntity>(string data)
        {
            return Serializer.Deserialize<TEntity>(data);
        }
    }
}
