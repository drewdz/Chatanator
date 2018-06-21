using Newtonsoft.Json;
using System.IO;

namespace PE.Framework.Serialization
{
    public static class Serializer
    {
        public static string Serialize<TEntity>(TEntity entity)
        {
            var serializer = new JsonSerializer();
            using (StringWriter sr = new StringWriter())
            {
                serializer.Serialize(sr, entity);
                sr.Flush();
                return sr.ToString();
            }
        }

        public static void Serialize<TEntity>(TEntity entity, Stream stream)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(Serialize(entity));
                writer.Flush();
            }
        }

        public static TEntity Deserialize<TEntity>(string json)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StringReader(json))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<TEntity>(reader);
                }
            }
        }

        public static object Deserialize(System.Type entityType, string json)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StringReader(json))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize(reader, entityType);
                }
            }
        }
    }
}
