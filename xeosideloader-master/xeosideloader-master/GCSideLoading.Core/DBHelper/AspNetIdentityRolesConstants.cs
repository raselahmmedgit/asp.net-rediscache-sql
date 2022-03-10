using AspNetCore.Identity.DocumentDb;
using GCSideLoading.Core.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.DBHelper
{
    public class AspNetIdentityRolesConstants
    {
        public static List<DocumentDbIdentityRole> MasterAdmin = new List<DocumentDbIdentityRole>
        {
             new DocumentDbIdentityRole
             {
                  Id = "c42ef2f4-80c2-47d7-a552-b519915413f4",
                  Name =  "MasterAdmin",
                  NormalizedName = "masteradmin",
                  Claims= null,                  
             }
        };
        public static List<DocumentDbIdentityRole> Admin = new List<DocumentDbIdentityRole>
        {
             new DocumentDbIdentityRole
             {
                  Id = "d303db56-30d8-49c6-9710-7c20b6f25bf2",
                  Name =  "Admin",
                  NormalizedName = "admin",
                  Claims= null,
             }
        };
        public static List<DocumentDbIdentityRole> Player = new List<DocumentDbIdentityRole>
        {
             new DocumentDbIdentityRole
             {
                  Id = "4c7e9852-d30b-43b6-8575-6490e4fd291f",
                  Name =  "Player",
                  NormalizedName = "player",
                  Claims= null,
             }
        };
    }
}
