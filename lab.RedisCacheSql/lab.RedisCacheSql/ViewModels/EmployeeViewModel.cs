using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab.RedisCacheSql.ViewModels
{
    public class EmployeeViewModel
    {
        [Key]
        public int EmployeeId { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(200)]
        public string LastName { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(200)]
        public string EmailAddress { get; set; }

        [DisplayName("Phone")]
        [Required(ErrorMessage = "Phone is required")]
        [MaxLength(200)]
        public string PhoneNumber { get; set; }

        [DisplayName("Present Address")]
        [Required(ErrorMessage = "Present Address is required")]
        [MaxLength(200)]
        public string PresentAddress { get; set; }

        [NotMapped]
        public string FullName => (FirstName + " " + LastName).Trim();
    }
}
