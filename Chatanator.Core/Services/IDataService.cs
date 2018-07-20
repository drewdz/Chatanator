using PE.Framework.DataProvider;
using PE.Provider.Data.Realm;

namespace Chatanator.Core.Services
{
    public interface IDataService : IDataProvider
    {
        DataProvider Provider { get; }
    }
}
