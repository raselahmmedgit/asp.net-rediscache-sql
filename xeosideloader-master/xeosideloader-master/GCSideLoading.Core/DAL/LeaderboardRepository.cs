using GCSideLoading.Core.EntityModel;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCSideLoading.Core.DAL
{
    public class LeaderboardRepository : BaseRepository<GCLeaderboard>
    {
        public LeaderboardRepository() : base(typeof(GCLeaderboard).Name)
        {

        }

        public bool checkIfItemExist(GCLeaderboard leaderboard)
        {
            try
            {
                if (string.IsNullOrEmpty(leaderboard.DataType))
                {
                    return documentclient.CreateDocumentQuery<GCLeaderboard>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == leaderboard.Cid && c.Gid == leaderboard.Gid && c.Sid == leaderboard.Sid).AsEnumerable().Any();

                }
                else
                {
                    return documentclient.CreateDocumentQuery<GCLeaderboard>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == leaderboard.Cid && c.Gid == leaderboard.Gid && c.Sid == leaderboard.Sid && c.DataType == leaderboard.DataType).AsEnumerable().Any();

                }
            }
            catch(Exception)
            {
                throw;
            }
        }
        public bool checkIfItemExistByRedisKey(GCLeaderboard leaderboard)
        {
            try
            {
                return documentclient.CreateDocumentQuery<GCLeaderboard>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.MappedRedisKey == leaderboard.MappedRedisKey).AsEnumerable().Any();

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
