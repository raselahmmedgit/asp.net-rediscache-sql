using GCSideLoading.Core.EntityModel;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCSideLoading.Core.DAL
{
    class GameDataRepository : BaseRepository<GCGameData>
    {
        public GameDataRepository() : base(typeof(GCGameData).Name)
        {

        }
        public bool checkIfItemExist(GCGameData gameData)
        {
            try
            {
                if (string.IsNullOrEmpty(gameData.Sid) && string.IsNullOrEmpty(gameData.DataType)
                    && string.IsNullOrEmpty(gameData.CardId) && string.IsNullOrEmpty(gameData.SubmitedCardAction))
                {
                    return documentclient.CreateDocumentQuery<GCGameData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == gameData.Cid && c.Gid == gameData.Gid).AsEnumerable().Any();

                }
                else if (string.IsNullOrEmpty(gameData.DataType)
                    && string.IsNullOrEmpty(gameData.CardId) && string.IsNullOrEmpty(gameData.SubmitedCardAction))
                {
                    return documentclient.CreateDocumentQuery<GCGameData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == gameData.Cid && c.Gid == gameData.Gid && c.Sid == gameData.Sid).AsEnumerable().Any();

                }
                else if (string.IsNullOrEmpty(gameData.CardId) && string.IsNullOrEmpty(gameData.SubmitedCardAction))
                {
                    return documentclient.CreateDocumentQuery<GCGameData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == gameData.Cid && c.Gid == gameData.Gid && c.Sid == gameData.Sid && c.DataType == gameData.DataType).AsEnumerable().Any();

                }
                else if (string.IsNullOrEmpty(gameData.SubmitedCardAction))
                {
                    return documentclient.CreateDocumentQuery<GCGameData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == gameData.Cid && c.Gid == gameData.Gid && c.Sid == gameData.Sid && c.DataType == gameData.DataType && c.CardId == gameData.CardId).AsEnumerable().Any();
                }
                else
                {
                    return documentclient.CreateDocumentQuery<GCGameData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                   new FeedOptions
                   {
                       MaxItemCount = -1
                   }).Where(c => c.Cid == gameData.Cid && c.Gid == gameData.Gid && c.Sid == gameData.Sid && c.DataType == gameData.DataType && c.CardId == gameData.CardId && c.SubmitedCardAction == gameData.SubmitedCardAction).AsEnumerable().Any();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool checkIfItemExistByRedisKey(GCGameData gameData)
        {
            try
            {
                return documentclient.CreateDocumentQuery<GCGameData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.MappedRedisKey == gameData.MappedRedisKey).AsEnumerable().Any();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
