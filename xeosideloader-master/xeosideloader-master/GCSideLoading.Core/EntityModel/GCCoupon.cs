using GCSideLoading.Core.EntityModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.EnitityModel
{
    public class GCCoupon
    {
        public GCCoupon()
        {

        }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string image { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }
        [JsonProperty(PropertyName = "order")]
        public string order { get; set; }
        [JsonProperty(PropertyName = "category")]
        public string category { get; set; }
        [JsonProperty(PropertyName = "gid")]
        public string gid { get; set; }
        [JsonProperty(PropertyName = "start")]
        public DateTime start { get; set; }
        [JsonProperty(PropertyName = "end")]
        public DateTime end { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public int? amount { get; set; }
        [JsonProperty(PropertyName = "priority")]
        public int priority { get; set; }
        [JsonProperty(PropertyName = "expiresAfter")]
        public int? expiresAfter { get; set; }

        [JsonProperty(PropertyName = "redirectUrl")]
        public string redirectUrl { get; set; }
        [JsonProperty(PropertyName = "redemptionCodeData")]
        public RedemptionCode redemptionCodeData { get; set; }
        [JsonProperty(PropertyName = "dataCapture")]
        public GCDataCaptureOption dataCapture { get; set; }
        [JsonProperty(PropertyName = "textNotificationEnabled")]
        public bool textNotificationEnabled { get; set; }
        [JsonProperty(PropertyName = "textNotificationMessage")]
        public string textNotificationMessage { get; set; }

    }
}

