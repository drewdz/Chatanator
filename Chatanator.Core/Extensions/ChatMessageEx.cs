using Chatanator.Core.Models;
using PE.Framework.Helpers;
using PE.Plugins.PubnubChat.Models;
using PE.Provider.Data.Realm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chatanator.Core.Extensions
{
    public static class ChatMessageEx
    {
        public static ChatMessage GetMessageById(this DataProvider provider, string messageId)
        {
            var dbMessage = provider.Database.All<ChatMessageRealm>().FirstOrDefault(m => m.MessageId.Equals(messageId));
            return (dbMessage == null) ? null : ObjectMapper.MapData<ChatMessageRealm, ChatMessage>(dbMessage);
        }

        public static List<ChatMessage> GetMessagesUser(this DataProvider provider, string userId, long timestamp = 0)
        {
            using (var database = provider.Database)
            {
                var dbMessages = database.All<ChatMessageRealm>().Where(m => (m.FromUser.Equals(userId) || m.ToUser.Equals(userId)) && m.TimeStamp >= timestamp).ToList();
                if ((dbMessages == null) || (dbMessages.Count == 0)) return null;
                return dbMessages.Select(m => ObjectMapper.MapData<ChatMessageRealm, ChatMessage>(m)).ToList();
            }
        }

        public static void Save(this ChatMessage message, DataProvider provider)
        {
            using (var database = provider.Database)
            {
                var dbMessage = database.All<ChatMessageRealm>().FirstOrDefault(m => m.MessageId.Equals(message.MessageId));
                if (dbMessage == null)
                {
                    dbMessage = ObjectMapper.MapData<ChatMessage, ChatMessageRealm>(message);
                    database.Write(() => database.Add(dbMessage));
                }
                else
                {
                    database.Write(() =>
                    {
                        dbMessage.FromUser = message.FromUser;
                        dbMessage.GroupId = message.GroupId;
                        dbMessage.PayloadType = message.PayloadType;
                        dbMessage.RawPayload = message.RawPayload;
                        dbMessage.TimeStamp = message.TimeStamp;
                        dbMessage.ToUser = message.ToUser;
                        dbMessage.Type = message.Type;
                    });
                }
            }
        }

        public static void Delete(this ChatMessage message, DataProvider provider)
        {
            var dbMessage = provider.Database.All<ChatMessageRealm>().FirstOrDefault(m => m.MessageId.Equals(message.MessageId));
            if (dbMessage == null) return;
            provider.Database.Write(() => provider.Database.Remove(dbMessage));
        }
    }
}
