using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.EnitityModel
{
    public class ClientProfile : Base
    {
        public ClientProfile()
        {

        }
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "clientName")]
        public string ClientName { get; set; }
        [JsonProperty(PropertyName = "gameUrlSlug")]
        public string GameUrlSlug { get; set; }
        [JsonProperty(PropertyName = "emailAddress")]
        public string EmailAddress { get; set; }
        [JsonProperty(PropertyName = "phoneNo")]
        public string PhoneNo { get; set; }
        [JsonProperty(PropertyName = "phoneCode")]
        public string PhoneCode { get; set; }
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "zipCode")]
        public string ZipCode { get; set; }
        [JsonProperty(PropertyName = "appUserId")]
        public string AppUserId { get; set; }
        [JsonProperty(PropertyName = "logoImage")]
        public string LogoImage { get; set; }
    }
}

