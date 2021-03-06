﻿using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using netDumbster.smtp;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Serilog; 

namespace EndpointServer
{
    class Program
    {
        static SimpleSmtpServer mail_server; 

        static void Main(string[] args)
        {


            var mail = Task.Run(() => MailServer()); 
            var ftp = Task.Run(() => FTPServer()); 

            Console.ReadLine();
        }

        private static Task MailServer()
        {
            int port = 25;

            //SMTP endpoint
            mail_server = SimpleSmtpServer.Start(port);
            mail_server.MessageReceived += (sender, mail) =>
            {
                // Get message body.
                var head = mail.Message.FromAddress;
                var body = mail.Message.MessageParts[0].BodyData;

                Console.WriteLine("Got Mail: " + head + "\n" + body);
            };
            Console.WriteLine("SMTP server open on " + port);
            return Task.CompletedTask; 
        }

        private static Task FTPServer()
        {

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            // Setup dependency injection
            var services = new ServiceCollection();

            // Add Serilog as logger provider
            services.AddLogging(lb => lb.AddSerilog());

            string CurrentPath = AppContext.BaseDirectory;
            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = CurrentPath);  //Path.Combine(Path.GetTempPath(), "TestFtpServer"));

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => { opt.ServerAddress = "127.0.0.1"; opt.Port = 21; opt.PasvAddress = "127.0.0.1"; opt.PasvMinPort = 1100; opt.PasvMaxPort = 1200; } );

            // Build the service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {

                // Initialize the FTP server
                var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                var cts = new CancellationTokenSource();

                // Start the FTP server
                ftpServerHost.StartAsync(cts.Token);

                Console.WriteLine("Press ENTER/RETURN to close the test application.");
                Console.ReadLine();

                // Stop the FTP server
                ftpServerHost.StopAsync(cts.Token).Wait();
            }

            return Task.CompletedTask; 
        }
    }
}
