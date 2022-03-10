using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EnitityModel
{
    public class Client
    {
        public Client()
        {
            ClientProfile = new ClientProfile();
            //Questions = new List<Question>();
            //GameCoupons = new List<GameCoupon>();
            //ClientGameSubscriptions = new List<ClientGameSubscription>();
        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "appUserId")]
        public string AppUserId { get; set; }
        [JsonProperty(PropertyName = "clientProfile")]
        public ClientProfile ClientProfile { get; set; }
        //[JsonProperty(PropertyName = "questions")]
        //public ICollection<Question> Questions { get; set; }
        //[JsonProperty(PropertyName = "gameCoupons")]
        //public ICollection<GameCoupon> GameCoupons { get; set; }
        //[JsonProperty(PropertyName = "clientGameSubscriptions")]
        //public ICollection<ClientGameSubscription> ClientGameSubscriptions { get; set; }
    }
}
