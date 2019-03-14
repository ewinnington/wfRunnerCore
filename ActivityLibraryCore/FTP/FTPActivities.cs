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
            var request = FtpWebRequest.CreateDefault(new Uri(@"ftp://localhost/EndpointServer.runtimeconfig.json"));
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            MemoryStream targetStream;
            using (targetStream = new MemoryStream())
            {
                //read from the input stream in 32K chunks
                //and save to output stream
                const int bufferLen = 32768;
                byte[] buffer = new byte[bufferLen];
                int count = 0;
                while ((count = stream.Read(buffer, 0, bufferLen)) > 0)
                {
                    targetStream.Write(buffer, 0, count);
                }

                targetStream.Position = 0;
                Console.WriteLine(targetStream.GetBuffer());
            }
        }
    }
}

