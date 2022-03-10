using GCSideLoading.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCSideLoading.Web.Helper
{
    public class ErrorPageViewModel
    {
        public ErrorPageViewModel()
        {
            ErrorType = MessageHelper.MessageTypeDanger;
            ErrorMessage = MessageHelper.Error;
        }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
