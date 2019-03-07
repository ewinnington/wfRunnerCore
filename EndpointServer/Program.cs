using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using netDumbster.smtp;
using System;
using System.IO;
using System.Threading;

namespace EndpointServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 25;

            //SMTP endpoint
            var mail_server = SimpleSmtpServer.Start(port);
            mail_server.MessageReceived += (sender, mail) =>
            {
                // Get message body.
                var head = mail.Message.MessageParts[0].HeaderData;
                var body = mail.Message.MessageParts[0].BodyData;

                Console.WriteLine("Got Mail: " + head + "\n" + body);
            };
            Console.WriteLine("SMTP server open on " + port);


            Thread t = new Thread(FTPServer);
            t.Start();

            Console.ReadLine();
        }

        private static void FTPServer(object obj)
        {
            // Setup dependency injection
            var services = new ServiceCollection();


            string CurrentPath = AppContext.BaseDirectory;
            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = "Z:\\Data");  //Path.Combine(Path.GetTempPath(), "TestFtpServer"));

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "127.0.0.1");

            // Build the service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {

                // Initialize the FTP server
                var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                var cts = new CancellationTokenSource();

                // Start the FTP server
                ftpServerHost.StartAsync(cts.Token).Wait();

                Console.WriteLine("Press ENTER/RETURN to close the test application.");
                Console.ReadLine();

                // Stop the FTP server
                ftpServerHost.StopAsync(cts.Token).Wait();
            }
        }
    }
}
