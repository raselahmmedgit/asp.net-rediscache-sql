using GCSideLoading.Core.EnitityModel;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static GCSideLoading.Core.Enums;

namespace GCSideLoading.Core.DAL
{
    public class GCSideLoadingDbRepository
    {
        private static IConfiguration _configuration;   
        private static string DatabaseId = null;
        private static DocumentClient _gcSideLoadingClient;

        public GCSideLoadingDbRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            DatabaseId = _configuration["GCSideLoadingDbConnection:DatabaseId"];
        }

        public GCSideLoadingDbRepository()
        {

        }

        private static void Initialize()
        {
            _gcSideLoadingClient = new DocumentClient(new Uri(_configuration["GCSideLoadingDbConnection:EndPointUrl"]), _configuration["GCSideLoadingDbConnection:AuthKey"]);

            if (_configuration["GCSideLoadingConfig:IsDatabaseCreate"] != null && bool.Parse(_configuration["GCSideLoadingConfig:IsDatabaseCreate"]))
            {
                CreateDatabaseIfNotExistsAsync().Wait();
            }
            if (_configuration["GCSideLoadingConfig:IsTableCreate"] != null && bool.Parse(_configuration["GCSideLoadingConfig:IsTableCreate"]))
            {
                //CreateIdentityUsersAndRoles().Wait();
                CreateTablesIfNotExists().Wait();
            }
            if (_configuration["GCSideLoadingConfig:IsDemoDataInsert"] != null && bool.Parse(_configuration["GCSideLoadingConfig:IsDemoDataInsert"]))
            {
                //InsertDemolDataAsync().Wait();
            }
        }
        public static DocumentClient GetDocumentClient()
        {
            if (_gcSideLoadingClient == null)
                Initialize();
            return _gcSideLoadingClient;
        }
        public static void CreateDocumentClient()
        {
            if (_gcSideLoadingClient == null)
                Initialize();
        }
        public static string GetDatabaseId()
        {
            return DatabaseId;
        }
        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {                
                await _gcSideLoadingClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
                
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _gcSideLoadingClient.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateIdentityUsersAndRoles()
        {
            try
            {
                // Does the Collection exist?
                await _gcSideLoadingClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, _configuration["GCSideLoadingConfig:AspNetIdentityUsers"]), new RequestOptions { OfferThroughput = 1000 });                                
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collection = new DocumentCollection() { Id = _configuration["GCSideLoadingConfig:AspNetIdentityUsers"] };
                    await _gcSideLoadingClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(DatabaseId), collection, new RequestOptions { OfferThroughput = 1000 });                    
                }
            }

            //try
            //{
            //    // Does the Collection exist?
            //    await _gcSideLoadingClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, _configuration["ConferenceGamingConfig:AspNetIdentityRoles"]), new RequestOptions { OfferThroughput = 1000 });
            //}
            //catch (DocumentClientException ex)
            //{
            //    if (ex.StatusCode == HttpStatusCode.NotFound)
            //    {
            //        DocumentCollection collection = new DocumentCollection() { Id = _configuration["GCSideLoadingConfig:AspNetIdentityRoles"] };
            //        await _gcSideLoadingClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(DatabaseId), collection, new RequestOptions { OfferThroughput = 1000 });
            //    }
            //}            
        }

        private static async Task CreateTablesIfNotExists()
        {
            //new ClientProfileRepository().Initialize();
            new LeaderboardRepository().Initialize();
            new GameDataRepository().Initialize();
            new AwardedCouponRepository().Initialize();
            new GCMiscRepository().Initialize();

            await Task.Yield();
        }

      
        private static async Task InsertDemolDataAsync()
        {
            #region Client Profile
            ClientProfileRepository clientProfileRepository = new ClientProfileRepository();
            var clientProfileExist = await clientProfileRepository.GetItemAsync("3116bb7d-ad48-4c5e-8630-a2aa046a9e64");
            if (clientProfileExist == null)
            {
                ClientProfile clientProfile = new ClientProfile
                {
                    Id = "3116bb7d-ad48-4c5e-8630-a2aa046a9e64",
                    FirstName = "ABC",
                    LastName = "Corporation",
                    EmailAddress = "admin@abc.com",
                    CreatedDate = DateTime.UtcNow
                };
                await clientProfileRepository.CreateItemAsync(clientProfile);
            }
            #endregion

            
        }
    }
}
