using AspNetCore.Identity.DocumentDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EnitityModel
{
    public class AspNetIdentityUsers
    {
        public AspNetIdentityUsers()
        {

        }
        [JsonProperty(PropertyName = "IsSystemAdmin")]
        public bool IsSystemAdmin { get; set; }
        [JsonProperty(PropertyName = "DisplayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "normalizedUserName")]
        public string NormalizedUserName { get; set; }
        [JsonProperty(PropertyName = "normalizedEmail")]
        public string NormalizedEmail { get; set; }
        [JsonProperty(PropertyName = "isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty(PropertyName = "isPhoneNumberConfirmed")]
        public string IsPhoneNumberConfirmed { get; set; }
        [JsonProperty(PropertyName = "passwordHash")]
        public string PasswordHash { get; set; }
        [JsonProperty(PropertyName = "SecurityStamp")]
        public string SecurityStamp { get; set; }
        [JsonProperty(PropertyName = "isTwoFactorAuthEnabled")]
        public bool IsTwoFactorAuthEnabled { get; set; }
        [JsonProperty(PropertyName = "logins")]
        public string[] Logins { get; set; }
        [JsonProperty(PropertyName = "roles")]
        public ICollection<DocumentDbIdentityRole> Roles { get; set; }
        [JsonProperty(PropertyName = "claims")]
        public string[] Claims { get; set; }
        [JsonProperty(PropertyName = "lockoutEnabled")]
        public bool LockoutEnabled { get; set; }
        [JsonProperty(PropertyName = "lockoutEndDate")]
        public string LockoutEndDate { get; set; }
        [JsonProperty(PropertyName = "accessFailedCount")]
        public int AccessFailedCount { get; set; }
        [JsonProperty(PropertyName = "authenticatorKey")]
        public string AuthenticatorKey { get; set; }
        [JsonProperty(PropertyName = "recoveryCodes")]
        public string RecoveryCodes { get; set; }
        [JsonProperty(PropertyName = "documentType")]
        public string DocumentType { get; set; }
        //[JsonProperty(PropertyName = "isPlayer")]
        //public bool IsPlayer { get; set; }
        //[JsonProperty(PropertyName = "playerProfile")]
        //public PlayerProfile PlayerProfile { get; set; }

    }
}
