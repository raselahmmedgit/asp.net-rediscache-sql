using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class HashListData
    {
        [JsonProperty(PropertyName = "key")]
        public string key { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string value { get; set; }
    }
}
