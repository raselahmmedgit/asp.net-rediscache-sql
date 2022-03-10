using AspNetCore.Identity.DocumentDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.EntityModel
{
    public class ApplicationUser : DocumentDbIdentityUser<DocumentDbIdentityRole>
    {
        public bool IsSystemAdmin { get; set; }
        public string DisplayName { get; set; }
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                return Email;
            }
            return DisplayName;
        }
        


    }
}
