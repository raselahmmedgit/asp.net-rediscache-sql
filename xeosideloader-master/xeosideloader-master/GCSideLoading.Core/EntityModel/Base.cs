using Newtonsoft.Json;
using System;
using static GCSideLoading.Core.Enums;

namespace GCSideLoading.Core.EnitityModel
{
    public class Base
    {
        public Base()
        {
            
        }
        
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }
        [JsonProperty(PropertyName = "updatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [JsonProperty(PropertyName = "updatedBy")]
        public string UpdatedBy { get; set; }
        [JsonProperty(PropertyName = "deletedDate")]
        public DateTime? DeletedDate { get; set; }
        [JsonProperty(PropertyName = "deletedBy")]
        public string DeletedBy { get; set; }
        [JsonProperty(PropertyName = "isDeleted")]
        public bool IsDeleted { get; set; }
        public void SetEntityStateInfo(EntityState entityState, string userId)
        {
            if (entityState == EntityState.Added)
            {
                CreatedDate = DateTime.UtcNow;
                CreatedBy = userId;
            }
            else if (entityState == EntityState.Modified)
            {
                UpdatedDate = DateTime.UtcNow;
                UpdatedBy = userId;
            }
            else if (entityState == EntityState.Deleted)
            {
                DeletedDate = DateTime.UtcNow;
                DeletedBy = userId;
            }
        }

    }
}
