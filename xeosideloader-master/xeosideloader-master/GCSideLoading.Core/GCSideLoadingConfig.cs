using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core
{
    public class GCSideLoadingConfig
    {
        public string AspNetIdentityUsers { get; set; }
        public string AspNetIdentityRoles { get; set; }
        public bool IsDatabaseCreate { get; set; }
        public bool IsTableCreate { get; set; }
        public bool IsMasterDataInsert { get; set; }
    }
}
