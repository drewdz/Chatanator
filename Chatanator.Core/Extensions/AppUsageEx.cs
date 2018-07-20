using Chatanator.Core.Models;
using Chatanator.Core.Services;
using PE.Framework.Helpers;
using PE.Provider.Data.Realm;
using System.Linq;

namespace Chatanator.Core.Extensions
{
    public static class AppUsageEx
    {
        public static AppUsage GetAppUsage(this IDataService provider)
        {
            var usageR = ((DataProvider)provider).Database.All<AppUsageRealm>().FirstOrDefault();
            if (usageR == null) return null;
            return ObjectMapper.MapData<AppUsageRealm, AppUsage>(usageR);
        }

        public static void Save(this AppUsage usage, IDataService provider)
        {
            //  get database instance
            using (var database = ((DataProvider)provider).Database)
            {
                var usageR = database.All<AppUsageRealm>().FirstOrDefault();
                if (usageR == null)
                {
                    usageR = ObjectMapper.MapData<AppUsage, AppUsageRealm>(usage);
                    database.Write(() => database.Add(usageR, true));
                }
                else
                {
                    database.Write(() =>
                    {
                        usageR.LastActivity = usage.LastActivity;
                    });
                }
            }
        }
    }
}
