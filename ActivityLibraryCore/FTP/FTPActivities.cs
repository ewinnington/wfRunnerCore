using CoreWf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace ActivityLibraryCore.FTP
{
    public sealed class DownloadFile : CodeActivity
    {
        [RequiredArgument]
        [Description("Full path with server name: 'ftp://localhost/EndpointServer.runtimeconfig.json'")]
        public InArgument<string> RemoteFilePath {get; set;}

        [RequiredArgument]
        [Description("Content as string of downloaded files - assumes UTF-8 encoded")]
        public OutArgument<string> FileContents { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(RemoteFilePath.Get(context));
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
                FileContents.Set(context, Encoding.UTF8.GetString(targetStream.GetBuffer()));
            }
        }
    }
}

