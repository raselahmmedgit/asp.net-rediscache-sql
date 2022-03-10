using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class UserAnswer
    {
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
    }
}
