using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.EntityModel
{
    public class RedemptionCode
    {
        public RedemptionCode()
        {

        }

        [JsonProperty(PropertyName = "prefix")]
        public string prefix { get; set; }
        [JsonProperty(PropertyName = "length")]
        public int? length { get; set; }
        [JsonProperty(PropertyName = "pregenerated")]
        public bool pregenerated { get; set; }
    }
}
