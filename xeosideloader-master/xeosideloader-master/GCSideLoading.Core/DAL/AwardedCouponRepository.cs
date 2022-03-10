using GCSideLoading.Core.EnitityModel;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCSideLoading.Core.DAL
{
    class AwardedCouponRepository : BaseRepository<GCAwardedCoupon>
    {
        public AwardedCouponRepository() : base(typeof(GCAwardedCoupon).Name)
        {

        }
        public bool checkIfItemExist(GCAwardedCoupon awardedCoupon)
        {
            try
            {
                if (string.IsNullOrEmpty(awardedCoupon.sid))
                {
                    return documentclient.CreateDocumentQuery<GCAwardedCoupon>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == awardedCoupon.Cid && c.Gid == awardedCoupon.Gid).AsEnumerable().Any();

                }
                else
                {
                    return documentclient.CreateDocumentQuery<GCAwardedCoupon>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.Cid == awardedCoupon.Cid && c.Gid == awardedCoupon.Gid && c.sid == awardedCoupon.sid).AsEnumerable().Any();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool checkIfItemExistByRedisKey(GCAwardedCoupon awardedCoupon)
        {
            try
            {
                return documentclient.CreateDocumentQuery<GCAwardedCoupon>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                     new FeedOptions
                     {
                         MaxItemCount = -1
                     }).Where(c => c.MappedRedisKey == awardedCoupon.MappedRedisKey).AsEnumerable().Any();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
