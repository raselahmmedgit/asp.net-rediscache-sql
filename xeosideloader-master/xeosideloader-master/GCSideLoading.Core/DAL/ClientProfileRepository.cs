using GCSideLoading.Core.EnitityModel;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.DAL
{
    public class ClientProfileRepository : BaseRepository<Client>
    {
        public ClientProfileRepository() : base(typeof(Client).Name)
        {

        }

        public async Task<ClientProfile> GetItemAsync(string id)
        {
            try
            {
                var ClientProfiles = documentclient.CreateDocumentQuery<Client>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(c => c.Id == id)
                .Select(d => d.ClientProfile)
                .AsEnumerable();
                if (ClientProfiles != null)
                {
                    return ClientProfiles.FirstOrDefault();
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ClientProfile> GetItemAsync(Func<ClientProfile, bool> predicate)
        {
            try
            {
                var ClientProfiles = documentclient.CreateDocumentQuery<Client>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Select(d => d.ClientProfile)
                .AsEnumerable();
                if (ClientProfiles != null)
                {
                    return ClientProfiles.FirstOrDefault(predicate);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ClientProfile>> GetItemsAsync(Func<ClientProfile, bool> predicate)
        {
            try
            {
                var ClientProfiles = documentclient.CreateDocumentQuery<Client>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Select(d => d.ClientProfile)
                .ToList();

                if (ClientProfiles != null)
                {
                    return ClientProfiles.Where(predicate);
                }
                else
                {
                    return new List<ClientProfile>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<ClientProfile>> GetItemsAsync()
        {
            try
            {
                return documentclient.CreateDocumentQuery<Client>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Select(d => d.ClientProfile)
                .AsEnumerable();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<Document> CreateItemAsync(ClientProfile ClientProfile)
        {
            try
            {
                Client client = new Client();
                client.AppUserId = ClientProfile.AppUserId;
                Document document = await CreateItemAsync(client);
                if (!string.IsNullOrEmpty(document.Id))
                {
                    client.Id = document.Id;
                    ClientProfile.Id = document.Id;
                    client.ClientProfile = ClientProfile;
                    await UpdateItemAsync(client.Id, client);
                }
                return document;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Document> UpdateItemAsync(string id, ClientProfile ClientProfile)
        {
            try
            {
                Document document = new Document();
                var Client = await base.GetItemAsync(id);

                if (Client != null && Client.ClientProfile != null)
                {
                    Client.ClientProfile = ClientProfile;
                    await UpdateItemAsync(Client.Id, Client);
                    document.Id = ClientProfile.Id;
                }
                return document;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try
            {
                var Client = await base.GetItemAsync(id);

                if (Client != null && Client.ClientProfile != null)
                {
                    Client.ClientProfile.IsDeleted = true;
                    await UpdateItemAsync(Client.Id, Client);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetGameUrlSlugByClientProfileId(string clientProfileId)
        {
            try
            {
                var clientProfile = documentclient.CreateDocumentQuery<Client>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions
                    {
                        MaxItemCount = -1
                    }).Where(c => c.Id == clientProfileId).Select(c => new ClientProfile { GameUrlSlug = c.ClientProfile.GameUrlSlug, ClientName = c.ClientProfile.ClientName }).AsEnumerable().FirstOrDefault();
                if (clientProfile != null)
                {
                    if (string.IsNullOrEmpty(clientProfile.GameUrlSlug))
                    {
                        var gameUrlSlug = clientProfile.ClientName.Trim().Replace(" ", "-");
                        var client = await base.GetItemAsync(clientProfileId);
                        client.ClientProfile.GameUrlSlug = gameUrlSlug;
                        await UpdateItemAsync(client.Id, client);
                        return gameUrlSlug;
                    }
                    else
                    {
                        return clientProfile.GameUrlSlug.Trim().Replace(" ", "-");
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ClientProfile GetClientIdAndLogoByGameUrlSlug(string gameUrlSlug)
        {
            try
            {
                if (string.IsNullOrEmpty(gameUrlSlug))
                    return null;
                return documentclient.CreateDocumentQuery<Client>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions
                    {
                        MaxItemCount = -1
                    }).Where(c => c.ClientProfile.GameUrlSlug == gameUrlSlug.Trim()).Select(c => new ClientProfile() { Id = c.ClientProfile.Id, LogoImage = c.ClientProfile.LogoImage }).AsEnumerable().FirstOrDefault();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
