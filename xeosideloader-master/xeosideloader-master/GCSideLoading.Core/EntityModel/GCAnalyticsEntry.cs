using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EnitityModel
{
    public class GCAnalyticsEntry
    {
        public GCAnalyticsEntry()
        {
            
        }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        [JsonProperty(PropertyName = "uid")]
        public string uid { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }
        [JsonProperty(PropertyName = "gid")]
        public string gid { get; set; }
        [JsonProperty(PropertyName = "cid")]
        public string cid { get; set; }
        [JsonProperty(PropertyName = "sessionId")]
        public string sessionId { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }
        [JsonProperty(PropertyName = "platform")]
        public string platform { get; set; }
        [JsonProperty(PropertyName = "time")]
        public DateTime? time { get; set; }
        [JsonProperty(PropertyName = "admin")]
        public bool admin { get; set; }
        [JsonProperty(PropertyName = "userAgent")]
        public string userAgent { get; set; }
        [JsonProperty(PropertyName = "buildNum")]
        public string buildNum { get; set; }
    }
}
