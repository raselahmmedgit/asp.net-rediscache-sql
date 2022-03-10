using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.EntityModel
{
    public class GCUser
    {
        public GCUser()
        {

        }

        [JsonProperty(PropertyName = "uid")]
        public string uid { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }
        [JsonProperty(PropertyName = "phone")]
        public string phone { get; set; }
        [JsonProperty(PropertyName = "isNew")]
        public bool isNew { get; set; }
    }
}
