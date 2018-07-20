using Chatanator.Core.Models;
using Chatanator.Core.Services;
using PE.Framework.Helpers;
using PE.Plugins.PubnubChat.Models;
using PE.Provider.Data.Realm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chatanator.Core.Extensions
{
    public static class ChannelHistoryEx
    {
        public static ChannelHistory GetChannelHistoryByChannelId(this IDataService provider, string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("id");
            //  get database version
            var histR = ((DataProvider)provider).Database.All<ChannelHistoryRealm>().FirstOrDefault(h => h.ChannelId.Equals(id));
            if (histR == null) return null;
            return ObjectMapper.MapData<ChannelHistoryRealm, ChannelHistory>(histR);
        }

        public static IEnumerable<ChannelHistory> GetChannelHistoriesByTimestampGreater(this IDataService provider, long timestamp)
        {
            //  get database version
            var histsR = ((DataProvider)provider).Database.All<ChannelHistoryRealm>().Where(h => h.TimeStamp > timestamp).ToList();
            if (histsR == null) yield return null;
            foreach (var histR in histsR)
            {
                yield return ObjectMapper.MapData<ChannelHistoryRealm, ChannelHistory>(histR);
            }
        }

        public static void Save(this ChannelHistory hist, IDataService provider)
        {
            //  make sure we have an id
            if (string.IsNullOrEmpty(hist.Id)) hist.Id = Guid.NewGuid().ToString();
            //  get saved instance
            var histR = ((DataProvider)provider).Database.All<ChannelHistoryRealm>().FirstOrDefault(h => h.Id.Equals(hist.Id));
            if (histR == null)
            {
                //  convert
                histR = ObjectMapper.MapData<ChannelHistory, ChannelHistoryRealm>(hist);
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() => database.Add(histR, true));
                }
            }
            else
            {
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() =>
                    {
                        histR.LastMessageId = hist.LastMessageId;
                        histR.TimeStamp = hist.TimeStamp;
                    });
                }
            }
        }
    }
}
