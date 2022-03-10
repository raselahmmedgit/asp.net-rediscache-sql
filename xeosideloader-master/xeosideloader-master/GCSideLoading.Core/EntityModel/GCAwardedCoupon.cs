using GCSideLoading.Core.EntityModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.EnitityModel
{
    public class GCAwardedCoupon
    {
        public GCAwardedCoupon()
        {

        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "mappedRedisKey")]
        public string MappedRedisKey { get; set; }
        [JsonProperty(PropertyName = "redisKeyType")]
        public string RedisKeyType { get; set; }
        [JsonProperty(PropertyName = "cid")]
        public string Cid { get; set; }
        [JsonProperty(PropertyName = "gid")]
        public string Gid { get; set; }
        [JsonProperty(PropertyName = "sid")]
        public string sid { get; set; }
        [JsonProperty(PropertyName = "delivered")]
        public bool delivered { get; set; }
        [JsonProperty(PropertyName = "index")]
        public int index { get; set; }
        [JsonProperty(PropertyName = "awardedAt")]
        public DateTime? awardedAt { get; set; }
        [JsonProperty(PropertyName = "gcCoupon")]
        public ICollection<GCCoupon> GCCoupons { get; set; }
        [JsonProperty(PropertyName = "listData")]
        public ICollection<String> ListData { get; set; }
        [JsonProperty(PropertyName = "sortedListData")]
        public ICollection<SortedListData> SortedListData { get; set; }
        [JsonProperty(PropertyName = "hashListData")]
        public ICollection<HashListData> HashListData { get; set; }
    }
}

