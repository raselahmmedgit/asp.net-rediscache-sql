using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class Award
    {
        public Award()
        {

        }
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }
        [JsonProperty(PropertyName = "award-status")]
        public string AwardStatus { get; set; }
    }
}
