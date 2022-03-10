using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.EntityModel
{
    public class GCLeader
    {
        public GCLeader()
        {

        }

        [JsonProperty(PropertyName = "uid")]
        public string uid { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }
        [JsonProperty(PropertyName = "position")]
        public int position { get; set; }
        [JsonProperty(PropertyName = "points")]
        public int points { get; set; }

    }
}
