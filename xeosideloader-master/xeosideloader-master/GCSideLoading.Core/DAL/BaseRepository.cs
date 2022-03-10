
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GCSideLoading.Core.EnitityModel;
using Microsoft.Azure.CosmosDB.BulkExecutor;

namespace GCSideLoading.Core.DAL
{
    public class BaseRepository<T> where T : class
    {
        public readonly string DatabaseId;
        public readonly string CollectionId;
        public readonly DocumentClient documentclient;
        public BaseRepository(string collectionId)
        {
            documentclient = GCSideLoadingDbRepository.GetDocumentClient();
            DatabaseId = GCSideLoadingDbRepository.GetDatabaseId();
            CollectionId = collectionId;

        }
        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await documentclient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IDocumentQuery<T> query = documentclient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();
                List<T> results = new List<T>();
                if (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<T> GetLastItemAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IDocumentQuery<T> query = documentclient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();
                List<T> results = new List<T>();
                if (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IDocumentQuery<T> query = documentclient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            try
            {
                IDocumentQuery<T> query = documentclient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Document> CreateItemAsync(T item)
        {
            try
            {
                return await documentclient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            try
            {
                return await documentclient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
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
                await documentclient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Initialize()
        {
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await documentclient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await documentclient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> ExecuteSqlQuery(string sqlQuery)
        {
            IDocumentQuery<T> query = documentclient.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(DatabaseId,CollectionId), sqlQuery,
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true }).AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }

        public async Task<IEnumerable<ReturnType>> ExecuteSqlQueryToGetList<ReturnType>(string sqlQuery) where ReturnType:class
        {
            IDocumentQuery<ReturnType> query = documentclient.CreateDocumentQuery<ReturnType>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), sqlQuery,
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true}).AsDocumentQuery();

            List<ReturnType> results = new List<ReturnType>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<ReturnType>());
            }
            return results;
        }

        public async Task<ReturnType> ExecuteSqlQueryToGetDocument<ReturnType>(string sqlQuery) where ReturnType : class
        {

            IDocumentQuery<ReturnType> query = documentclient.CreateDocumentQuery<ReturnType>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), sqlQuery,
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true }).AsDocumentQuery();

            List<ReturnType> results = new List<ReturnType>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<ReturnType>());
            }
            return results.FirstOrDefault();
        }

        public int TotalRowCount()
        {
            try
            {
                return documentclient.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                   new FeedOptions
                   {
                       MaxItemCount = -1
                   }).Count();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //private async Task RunBulkImportAsync()
        //{
        //    // Cleanup on start if set in config.


        //    try
        //    {
        //        DocumentCollection dataCollection = documentclient.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(DatabaseId))
        //        .Where(c => c.Id == CollectionId).AsEnumerable().FirstOrDefault();
        //        documentclient.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
        //        documentclient.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 9;

        //        BulkExecutor bulkExecutor = new BulkExecutor(documentclient, dataCollection);
        //        await bulkExecutor.InitializeAsync();
        //        // Prepare for bulk import.

        //        // Creating documents with simple partition key here.
        //        string partitionKeyProperty = dataCollection.PartitionKey.Paths[0].Replace("/", "");

        //        long numberOfDocumentsToGenerate = long.Parse(ConfigurationManager.AppSettings["NumberOfDocumentsToImport"]);
        //        int numberOfBatches = int.Parse(ConfigurationManager.AppSettings["NumberOfBatches"]);
        //        long numberOfDocumentsPerBatch = (long)Math.Floor(((double)numberOfDocumentsToGenerate) / numberOfBatches);

        //        // Set retry options high for initialization (default values).
        //        client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
        //        client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 9;

        //        IBulkExecutor bulkExecutor = new BulkExecutor(client, dataCollection);
        //        await bulkExecutor.InitializeAsync();

        //        // Set retries to 0 to pass control to bulk executor.
        //        client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 0;
        //        client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 0;

        //        BulkImportResponse bulkImportResponse = null;
        //        long totalNumberOfDocumentsInserted = 0;
        //        double totalRequestUnitsConsumed = 0;
        //        double totalTimeTakenSec = 0;

        //        var tokenSource = new CancellationTokenSource();
        //        var token = tokenSource.Token;

        //        for (int i = 0; i < numberOfBatches; i++)
        //        {
        //            // Generate JSON-serialized documents to import.

        //            List<string> documentsToImportInBatch = new List<string>();
        //            long prefix = i * numberOfDocumentsPerBatch;

        //            Trace.TraceInformation(String.Format("Generating {0} documents to import for batch {1}", numberOfDocumentsPerBatch, i));
        //            for (int j = 0; j < numberOfDocumentsPerBatch; j++)
        //            {
        //                string partitionKeyValue = (prefix + j).ToString();
        //                string id = partitionKeyValue + Guid.NewGuid().ToString();

        //                documentsToImportInBatch.Add(Utils.GenerateRandomDocumentString(id, partitionKeyProperty, partitionKeyValue));
        //            }

        //            // Invoke bulk import API.

        //            var tasks = new List<Task>();

        //            tasks.Add(Task.Run(async () =>
        //            {
        //                Trace.TraceInformation(String.Format("Executing bulk import for batch {0}", i));
        //                do
        //                {
        //                    try
        //                    {
        //                        bulkImportResponse = await bulkExecutor.BulkImportAsync(
        //                            documents: documentsToImportInBatch,
        //                            enableUpsert: true,
        //                            disableAutomaticIdGeneration: true,
        //                            maxConcurrencyPerPartitionKeyRange: null,
        //                            maxInMemorySortingBatchSize: null,
        //                            cancellationToken: token);
        //                    }
        //                    catch (DocumentClientException de)
        //                    {
        //                        Trace.TraceError("Document client exception: {0}", de);
        //                        break;
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Trace.TraceError("Exception: {0}", e);
        //                        break;
        //                    }
        //                } while (bulkImportResponse.NumberOfDocumentsImported < documentsToImportInBatch.Count);

        //                Trace.WriteLine(String.Format("\nSummary for batch {0}:", i));
        //                Trace.WriteLine("--------------------------------------------------------------------- ");
        //                Trace.WriteLine(String.Format("Inserted {0} docs @ {1} writes/s, {2} RU/s in {3} sec",
        //                    bulkImportResponse.NumberOfDocumentsImported,
        //                    Math.Round(bulkImportResponse.NumberOfDocumentsImported / bulkImportResponse.TotalTimeTaken.TotalSeconds),
        //                    Math.Round(bulkImportResponse.TotalRequestUnitsConsumed / bulkImportResponse.TotalTimeTaken.TotalSeconds),
        //                    bulkImportResponse.TotalTimeTaken.TotalSeconds));
        //                Trace.WriteLine(String.Format("Average RU consumption per document: {0}",
        //                    (bulkImportResponse.TotalRequestUnitsConsumed / bulkImportResponse.NumberOfDocumentsImported)));
        //                Trace.WriteLine("---------------------------------------------------------------------\n ");

        //                totalNumberOfDocumentsInserted += bulkImportResponse.NumberOfDocumentsImported;
        //                totalRequestUnitsConsumed += bulkImportResponse.TotalRequestUnitsConsumed;
        //                totalTimeTakenSec += bulkImportResponse.TotalTimeTaken.TotalSeconds;
        //            },
        //            token));

        //            /*
        //            tasks.Add(Task.Run(() =>
        //            {
        //                char ch = Console.ReadKey(true).KeyChar;
        //                if (ch == 'c' || ch == 'C')
        //                {
        //                    tokenSource.Cancel();
        //                    Trace.WriteLine("\nTask cancellation requested.");
        //                }
        //            }));
        //            */

        //            await Task.WhenAll(tasks);
        //        }

        //        Trace.WriteLine("Overall summary:");
        //        Trace.WriteLine("--------------------------------------------------------------------- ");
        //        Trace.WriteLine(String.Format("Inserted {0} docs @ {1} writes/s, {2} RU/s in {3} sec",
        //            totalNumberOfDocumentsInserted,
        //            Math.Round(totalNumberOfDocumentsInserted / totalTimeTakenSec),
        //            Math.Round(totalRequestUnitsConsumed / totalTimeTakenSec),
        //            totalTimeTakenSec));
        //        Trace.WriteLine(String.Format("Average RU consumption per document: {0}",
        //            (totalRequestUnitsConsumed / totalNumberOfDocumentsInserted)));
        //        Trace.WriteLine("--------------------------------------------------------------------- ");

        //        // Cleanup on finish if set in config.

        //        if (bool.Parse(ConfigurationManager.AppSettings["ShouldCleanupOnFinish"]))
        //        {
        //            Trace.TraceInformation("Deleting Database {0}", DatabaseName);
        //            await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseName));
        //        }

        //        Trace.WriteLine("\nPress any key to exit.");
        //        Console.ReadKey();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

           
        //}
    }
}
