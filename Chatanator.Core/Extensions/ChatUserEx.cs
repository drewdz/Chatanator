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
    public static class ChatUserEx
    {
        public static ChatUser GetChatUserById(this IDataService provider, string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("id");
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.ChatUserId.Equals(id));
            if (userR == null) return null;
            return ObjectMapper.MapData<ChatUserRealm, ChatUser>(userR);
        }

        public static ChatUser GetAppUser(this IDataService provider)
        {
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.AppUser);
            if (userR == null) return null;
            return ObjectMapper.MapData<ChatUserRealm, ChatUser>(userR);
        }

        public static IEnumerable<ChatUser> GetChatUsers(this IDataService provider)
        {
            var usersR = ((DataProvider)provider).Database.All<ChatUserRealm>().ToList();
            if ((usersR == null) || (usersR.Count == 0)) usersR = new List<ChatUserRealm>();
            foreach (var user in usersR)
            {
                yield return ObjectMapper.MapData<ChatUserRealm, ChatUser>(user);
            }
        }

        public static List<ChatUser> CreateUsers(this IDataService provider)
        {
            var list = new List<ChatUser>()
            {
                new ChatUser { ChatUserId = "90001", FirstName = "Tim", LastName = "Arnold" },
                new ChatUser { ChatUserId = "90002", FirstName = "Andrew", LastName = "D'Alton" },
                new ChatUser { ChatUserId = "90003", FirstName = "Dzmitry", LastName = "Kaliuzhny" },
                new ChatUser { ChatUserId = "90004", FirstName = "Aliaksandr", LastName = "Hertsyk" },
                new ChatUser { ChatUserId = "90005", FirstName = "Kyle", LastName = "Dahl" },
                new ChatUser { ChatUserId = "90006", FirstName = "Batman", LastName = "" }
            };
            //  save
            foreach (var user in list) user.Save(provider);
            //  done
            return list;
        }

        public static void Save(this ChatUser user, IDataService provider)
        {
            //  make sure we have an id
            if (string.IsNullOrEmpty(user.ChatUserId)) user.ChatUserId = Guid.NewGuid().ToString();
            //  get saved instance
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.ChatUserId.Equals(user.ChatUserId));
            if (userR == null)
            {
                //  convert to realm
                userR = ObjectMapper.MapData<ChatUser, ChatUserRealm>(user);
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() => database.Add(userR, true));
                }
            }
            else
            {
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() =>
                    {
                        userR.Email = user.Email;
                        userR.FirstName = user.FirstName;
                        userR.LastName = user.LastName;
                    });
                }
            }
        }

        public static void SaveAppUser(this ChatUser user, IDataService provider)
        {
            //  get saved instance
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.ChatUserId.Equals(user.ChatUserId));
            if (userR == null)
            {
                //  convert to realm
                userR = ObjectMapper.MapData<ChatUser, ChatUserRealm>(user);
                userR.AppUser = true;
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() => database.Add(userR, true));
                }
            }
            else
            {
                using (var database = ((DataProvider)provider).Database)
                {
                    database.Write(() =>
                    {
                        userR.Email = user.Email;
                        userR.FirstName = user.FirstName;
                        userR.LastName = user.LastName;
                        userR.AppUser = true;
                    });
                }
            }
        }
    }
}
