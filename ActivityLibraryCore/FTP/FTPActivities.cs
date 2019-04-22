using CoreWf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace ActivityLibraryCore.FTP
{
    public sealed class UploadFile : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {

        }
    }

    public sealed class DownloadFile : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(@"ftp://localhost/EndpointServer.runtimeconfig.json");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("anonymous", "abc@example.com");
            request.KeepAlive = false;

            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            MemoryStream targetStream;
            using (targetStream = new MemoryStream())
            {
                stream.CopyTo(targetStream);

                targetStream.Position = 0;
                Console.WriteLine(Encoding.UTF8.GetString(targetStream.GetBuffer()));
            }
        }
    }
}

