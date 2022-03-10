using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCSideLoading.Web.Models.AccountViewModels
{
    public class ExternalPlayerLoginConfirmationViewModel
    {
        [Required]
        public string UserName { get; set; }

        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsEmail { get; set; }

        //[Required]
        public string MobilePhone { get; set; }

        public bool IsMobilePhone { get; set; }

        public string PublishGameId { get; set; }
        public string GameId { get; set; }
        public string GameTitle { get; set; }
        public string RoleName { get; set; }
    }
}
