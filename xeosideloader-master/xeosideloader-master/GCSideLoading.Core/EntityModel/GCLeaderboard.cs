using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class GCLeaderboard
    {
        public GCLeaderboard()
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
        public string Sid { get; set; }
        [JsonProperty(PropertyName = "dataType")]
        public string DataType { get; set; }
        [JsonProperty(PropertyName = "userData")]
        public ICollection<GCUser> UserData { get; set; }
        [JsonProperty(PropertyName = "awardStatus")]
        public ICollection<Award> AwardStatus { get; set; }
        [JsonProperty(PropertyName = "listData")]
        public ICollection<String> ListData { get; set; }
        [JsonProperty(PropertyName = "sortedListData")]
        public ICollection<SortedListData> SortedListData { get; set; }
        [JsonProperty(PropertyName = "hashListData")]
        public ICollection<HashListData> HashListData { get; set; }

    }
}
