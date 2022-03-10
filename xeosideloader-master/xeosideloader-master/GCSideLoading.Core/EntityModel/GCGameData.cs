using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class GCGameData
    {
        public GCGameData()
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
        [JsonProperty(PropertyName = "cardId")]
        public string CardId { get; set; }
        [JsonProperty(PropertyName = "submitedCardAction")]
        public string SubmitedCardAction { get; set; }
        [JsonProperty(PropertyName = "users")]
        public ICollection<String> Users { get; set; }
        [JsonProperty(PropertyName = "userAnswers")]
        public ICollection<UserAnswer> UserAnswers { get; set; }
        [JsonProperty(PropertyName = "listData")]
        public ICollection<String> ListData { get; set; }
        [JsonProperty(PropertyName = "sortedListData")]
        public ICollection<SortedListData> SortedListData { get; set; }
        [JsonProperty(PropertyName = "hashListData")]
        public ICollection<HashListData> HashListData { get; set; }
    }
}
