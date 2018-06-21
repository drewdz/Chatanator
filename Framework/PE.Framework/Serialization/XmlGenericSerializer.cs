using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PE.Framework.Serialization
{
    public class XmlGenericSerializer : ISerializer, IDisposable
    {
        public string Serialize<TEntity>(TEntity entity)
        {
            var serializer = new XmlSerializer(typeof(TEntity));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, entity);
                var data = ms.ToArray();
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
        }

        public TEntity Deserialize<TEntity>(string data)
        {
            var serializer = new XmlSerializer(typeof(TEntity));
            return (TEntity)serializer.Deserialize(new StringReader(data));
        }

        public void Dispose()
        {
        }
    }
}
