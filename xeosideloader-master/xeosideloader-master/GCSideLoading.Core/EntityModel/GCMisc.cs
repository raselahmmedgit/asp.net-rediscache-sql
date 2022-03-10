using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class GCMisc
    {
        public GCMisc()
        {

        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "configDataType")]
        public string ConfigDataType { get; set; }
        [JsonProperty(PropertyName = "dataList")]
        public List<string> DataList { get; set; }
    }
}
