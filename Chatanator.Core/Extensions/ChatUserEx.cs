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
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.Id.Equals(id));
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
                new ChatUser { Id = "7E9703A8-F839-4689-A2FA-43FD63A0282D", FirstName = "Tim", LastName = "Arnold" },
                new ChatUser { Id = "5878440B-D5B1-4CBB-82C8-7BB5612D4DC5", FirstName = "Andrew", LastName = "D'Alton" },
                new ChatUser { Id = "E3B14153-C575-46AC-AB09-550AC7D3B121", FirstName = "Dzmitry", LastName = "Kaliuzhny" },
                new ChatUser { Id = "C850A7FB-505F-4F7C-8362-F3614EE2804E", FirstName = "Aliaksandr", LastName = "Hertsyk" },
                new ChatUser { Id = "87E45CAE-A355-448C-A319-9006F7119EB9", FirstName = "Kyle", LastName = "Dahl" }
            };
            //  save
            foreach (var user in list) user.Save(provider);
            //  done
            return list;
        }

        public static void Save(this ChatUser user, IDataService provider)
        {
            //  make sure we have an id
            if (string.IsNullOrEmpty(user.Id)) user.Id = Guid.NewGuid().ToString();
            //  get saved instance
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.Id.Equals(user.Id));
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
            var userR = ((DataProvider)provider).Database.All<ChatUserRealm>().FirstOrDefault(u => u.Id.Equals(user.Id));
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
