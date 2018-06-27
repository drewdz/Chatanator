using Chatanator.Core.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatanator.Core.Services
{
    public class CosmosDataService : ICosmosDataService
    {
        #region Constants

        private const string DB_NAME = "Chat";
        private const string KEY_URI = "https://chatanator.documents.azure.com:443/";
        private const string KEY_PRIMARY = "bWmpVLsUkkbiYW5qDYYlGHua2SEPp8pRwM9V40SnlNNLaOigs71vrmlYtXlNIaM1J0nYPq2GtkYzeNPj6N9CKQ==";

        #endregion Constants

        #region Fields

        private readonly DocumentClient _DocumentClient;

        private bool _Initialized = false;

        #endregion Fields

        #region Constructors

        public CosmosDataService()
        {
            _DocumentClient = new DocumentClient(new Uri(KEY_URI), KEY_PRIMARY);
            Initialize();
        }

        #endregion Constructors

        #region Init

        private async void Initialize()
        {
            var dbPath = string.Format("/dbs/{0}", DB_NAME);
            using (DocumentClient client = new DocumentClient(new Uri(KEY_URI), KEY_PRIMARY))
            {
                await client.CreateDocumentCollectionIfNotExistsAsync(dbPath, new DocumentCollection { Id = typeof(ChatUser).Name }, new RequestOptions { OfferThroughput = 400 });
                await client.CreateDocumentCollectionIfNotExistsAsync(dbPath, new DocumentCollection { Id = typeof(Channel).Name }, new RequestOptions { OfferThroughput = 400 });
                await client.CreateDocumentCollectionIfNotExistsAsync(dbPath, new DocumentCollection { Id = typeof(ChannelHistory).Name }, new RequestOptions { OfferThroughput = 400 });
                await client.CreateDocumentCollectionIfNotExistsAsync(dbPath, new DocumentCollection { Id = typeof(ChannelUser).Name }, new RequestOptions { OfferThroughput = 400 });
                await client.CreateDocumentCollectionIfNotExistsAsync(dbPath, new DocumentCollection { Id = typeof(ChatMessage).Name }, new RequestOptions { OfferThroughput = 400 });
            }
            _Initialized = true;
        }

        #endregion Init

        #region Operations

        #region Read

        public async Task<List<ChatUser>> GetChatUsersAsync()
        {
            return await GetItemsAsync<ChatUser>();
        }

        public async Task<List<Channel>> GetChannelsAsync()
        {
            return await GetItemsAsync<Channel>();
        }

        public async Task<List<ChannelUser>> GetChannelUsersAsync()
        {
            return await GetItemsAsync<ChannelUser>();
        }

        public async Task<List<ChannelHistory>> GetChannelHistoryAsync()
        {
            return await GetItemsAsync<ChannelHistory>();
        }

        private async Task<List<TItem>> GetItemsAsync<TItem>()
        {
            try
            {
                var query = _DocumentClient.CreateDocumentQuery<TItem>(UriFactory.CreateDocumentCollectionUri(DB_NAME, typeof(TItem).Name), new FeedOptions { MaxItemCount = -1 }).AsDocumentQuery();

                List<TItem> items = new List<TItem>();
                while (query.HasMoreResults)
                {
                    var result = await query.ExecuteNextAsync<TItem>();
                    items.AddRange(result);
                }

                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** CosmosDataService.GetItemsAsync - Exception: {0}", ex));
                return new List<TItem>();
            }
        }

        #endregion Read

        #region Add

        public async Task AddAsync<TItem>(TItem item)
        {
            await AddItemAsync(item);
        }

        public async Task AddAsync<TItem>(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                await AddItemAsync(item);
            }
        }

        private async Task AddItemAsync<TItem>(TItem item)
        {
            try
            {
                if (!_Initialized) return;
                var collectionName = typeof(TItem).Name;
                using (DocumentClient client = new DocumentClient(new Uri(KEY_URI), KEY_PRIMARY))
                {
                    DocumentCollection collection = await client.CreateDocumentCollectionIfNotExistsAsync(string.Format("/dbs/{0}", DB_NAME), new DocumentCollection { Id = collectionName }, new RequestOptions { OfferThroughput = 400 });
                    await _DocumentClient.CreateDocumentAsync(collection.DocumentsLink, item, null, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** CosmosDataService.AddItem - Exception: {0}", ex));
                throw ex;
            }
        }

        #endregion Add

        #region Delete

        public async Task DeleteAsync<TItem>(TItem item) where TItem : IIndexable
        {
            await DeleteItemAsync(item);
        }

        public async Task DeleteAsync<TItem>(IEnumerable<TItem> items) where TItem : IIndexable
        {
            foreach (var item in items)
            {
                await DeleteItemAsync(item);
            }
        }

        private async Task DeleteItemAsync<TItem>(TItem item) where TItem : IIndexable
        {
            if (!_Initialized) return;
            await _DocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DB_NAME, typeof(TItem).Name, item.Id));
        }

        #endregion Delete

        #region Update

        public async Task UpdateAsync<TItem>(TItem item) where TItem : IIndexable
        {
            await UpdateItemAsync(item);
        }

        public async Task UpdateItemAsync<TItem>(TItem item) where TItem : IIndexable
        {
            if (!_Initialized) return;
            await _DocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DB_NAME, typeof(TItem).Name, item.Id), item);
        }

        #endregion Update

        #endregion Operations
    }
}
