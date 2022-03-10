using System.ComponentModel.DataAnnotations;

namespace lab.RedisCacheSql.Models
{
    public class PatientProfile
    {
        [Key]
        public long PatientProfileId { get; set; }

        [StringLength(250)]
        public string? FirstName { get; set; }

        [StringLength(250)]
        public string? LastName { get; set; }

        [StringLength(250)]
        public string? PreferredName { get; set; }

        [StringLength(100)]
        public string? PrimaryPhone { get; set; }

        [StringLength(10)]
        public string? PrimaryPhoneCode { get; set; }

        public int? PrimaryPhoneCountryId { get; set; }

        [StringLength(100)]
        public string? OtherPhone { get; set; }

        [StringLength(10)]
        public string? OtherPhoneCode { get; set; }

        public int? OtherPhoneCountryId { get; set; }

        public bool EnableSmsNotification { get; set; }

        [StringLength(250)]
        public string? EmailAddress { get; set; }

        [StringLength(128)]
        public string? AppUserId { get; set; }

        public DateTime RegistrationDate { get; set; }

        //public DateTime? DateOfBirth { get; set; }
        public string? DateOfBirthStr { get; set; }

        public string? PatientPassword { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeactivated { get; set; }

    }
}
