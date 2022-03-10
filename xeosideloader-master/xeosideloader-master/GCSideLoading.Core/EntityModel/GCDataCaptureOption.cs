using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GCSideLoading.Core.EntityModel
{
    public class GCDataCaptureOption
    {
        public GCDataCaptureOption()
        {

        }

        [JsonProperty(PropertyName = "message")]
        public string message { get; set; }
        [JsonProperty(PropertyName = "askEmail")]
        public bool askEmail { get; set; }
        [JsonProperty(PropertyName = "askName")]
        public bool askName { get; set; }
        [JsonProperty(PropertyName = "askPhone")]
        public bool askPhone { get; set; }
        [JsonProperty(PropertyName = "displayTop")]
        public bool displayTop { get; set; }
    }
}
