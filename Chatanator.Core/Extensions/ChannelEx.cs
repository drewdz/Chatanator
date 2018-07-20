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
    public static class ChannelEx
    {
        public static Channel GetChannelById(this IDataService provider, string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("id");
            var channelR = ((DataProvider)provider).Database.All<ChannelRealm>().FirstOrDefault(c => c.Id.Equals(id));
            if (channelR == null) return null;
            //  convert from database
            return ObjectMapper.MapData<ChannelRealm, Channel>(channelR);
        }

        public static Channel GetChannelForUsers(this IDataService provider, string id1, string id2)
        {
            if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2)) throw new ArgumentException("ids");
            //  find channels with only these two users
            var channelsR = ((DataProvider)provider).Database.All<ChannelRealm>().Where(ch => (ch.ChannelType == 0) && !string.IsNullOrEmpty(ch.UsersKey) && ch.UsersKey.Contains(id1) && ch.UsersKey.Contains(id2)).ToList();
            //  convert
            var channels = new List<Channel>();
            foreach (var channelR in channelsR)
            {
                var channel = ObjectMapper.MapData<ChannelRealm, Channel>(channelR);
                channel.Users = channelR.UsersKey.Split(new char[] { ',' });
                channels.Add(channel);
            }
            return channels.FirstOrDefault(ch => ch.Users.Length == 2);
        }

        public static IEnumerable<Channel> GetChannels(this IDataService provider)
        {
            var channelsR = ((DataProvider)provider).Database.All<ChannelRealm>().ToList();
            if ((channelsR == null) || (channelsR.Count == 0)) yield return null;
            foreach (var channelR in channelsR)
            {
                var channel = ObjectMapper.MapData<ChannelRealm, Channel>(channelR);
                channel.ChannelType = (ChannelType)channelR.ChannelType;
                yield return channel;
            }
        }

        public static void Save(this Channel channel, IDataService provider)
        {
            
            //  get saved instance
            var channelR = ((DataProvider)provider).Database.All<ChannelRealm>().FirstOrDefault(c => c.Id.Equals(channel.Id));
            if (channelR == null)
            {
                //  convert to realm
                channelR = ObjectMapper.MapData<Channel, ChannelRealm>(channel);
                channelR.ChannelType = (int)channel.ChannelType;
                channelR.UsersKey = channel.UsersKey;
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() => database.Add(channelR, true));
                }
            }
            else
            {
                //  save
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() =>
                    {
                        channelR.ChannelType = (int)channel.ChannelType;
                        channelR.LastActivity = channel.LastActivity;
                        channelR.Name = channel.Name;
                        channelR.UsersKey = channel.UsersKey;
                    });
                }
            }
        }
    }
}
