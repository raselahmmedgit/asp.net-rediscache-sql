using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class SortedListData
    {
        [JsonProperty(PropertyName = "key")]
        public string key { get; set; }
        [JsonProperty(PropertyName = "value")]
        public double value { get; set; }
    }
}
