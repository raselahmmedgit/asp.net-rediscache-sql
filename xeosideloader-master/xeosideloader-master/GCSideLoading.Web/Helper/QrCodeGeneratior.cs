using GCSideLoading.Core.Helper;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GCSideLoading.Web.Helper
{
    public class QrCodeGeneratior
    {
        private readonly IHostingEnvironment _env;
        private readonly ILog _log;

        public QrCodeGeneratior(IHostingEnvironment env)
        {
            _env = env;
            _log = LogManager.GetLogger(typeof(QrCodeGeneratior));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishGameId"></param>
        public string GenerateQrCode(string gameUrl, string id)
        {
            string fileName = $"{id}.png";
            try
            {
                string filePath = $"{_env.WebRootPath}\\images\\qrcode\\{fileName}";
                //if (!Directory.Exists(filePath))
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(gameUrl, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    qrCodeImage.Save(filePath);
                }
            }
            catch(Exception ex)
            {
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "GenerateQrCode",id));
            }
            
            return fileName;
        }
    }
}
