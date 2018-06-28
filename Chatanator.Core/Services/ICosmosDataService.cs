using Chatanator.Core.Models;
using PE.Framework.DataProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatanator.Core.Services
{
    public interface ICosmosDataService
    {
        #region Operations

        Task<List<ChatUser>> GetChatUsersAsync();

        Task<List<Channel>> GetChannelsAsync();

        Task<List<ChannelHistory>> GetChannelHistoryAsync();


        Task AddAsync<TItem>(TItem item);

        Task AddAsync<TItem>(IEnumerable<TItem> items);

        Task DeleteAsync<TItem>(TItem item) where TItem : IIndexable;

        Task DeleteAsync<TItem>(IEnumerable<TItem> items) where TItem : IIndexable;

        Task UpdateAsync<TItem>(TItem item) where TItem : IIndexable;

        #endregion Operations
    }
}
