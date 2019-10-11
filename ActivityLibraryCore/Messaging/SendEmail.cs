using System.Activities;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowExtensionInterfaces;
using System.Activities.Tracking;

namespace ActivityLibraryCore.Messaging
{
    public sealed class SendEmail : CodeActivity
    {
        /// <summary>
        /// Destination: can be a semicolon separated list for multiple reciver
        /// </summary>
        public InArgument<string> To { get; set; }
        public InArgument<string> Subject { get; set; }
        public InArgument<string> Body { get; set; }
        public InArgument<bool> UseSSL { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                bool useSSL = context.GetValue(UseSSL);

                string[] RecieverList = To.Get(context).Split(';');
                var mailMessage = new System.Net.Mail.MailMessage();
                foreach (string address in RecieverList)
                {
                    if (address.Trim() != "") mailMessage.To.Add(address.Trim());
                }

                mailMessage.Subject = Subject.Get(context);
                mailMessage.Body = Body.Get(context);

                //Get email setting from extension
                IEmailSetting settings = context.GetExtension<IEmailSetting>();

                if (settings != null)
                {
                    var FromAddress = settings.GetAddress();
                    var Host = settings.GetHost();
                    var UserName = settings.GetUsername();
                    var Password = settings.GetPassword(); 

                    if(FromAddress == null | Host == null | UserName == null | Password == null)
                    {
                        var record = new CustomTrackingRecord("Warning");
                        record.Data.Add(new KeyValuePair<string, object>("Message", "E-mail extension not configured. Please make sure to run the workflow with the environment variables configured."));
                        context.Track(record);
                        return;
                    }

                        mailMessage.From = new System.Net.Mail.MailAddress(FromAddress);
                    var smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = Host;
                    smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
                    smtp.EnableSsl = useSSL;
                    smtp.Send(mailMessage);
                }
                else
                {
                    var record = new CustomTrackingRecord("Warning");
                    record.Data.Add(new KeyValuePair<string, object>("Message", "E-mail extension not found. Please make sure to run the workflow with a configured EmailSetting extension."));
                    context.Track(record);
                }
            }
            catch (Exception ex)
            {
                var record = new CustomTrackingRecord("Warning");
                record.Data.Add(new KeyValuePair<string, object>("Message", "Error while sending e-mail.\n" + ex.ToString()));
                context.Track(record);
            }
        }
    }
}
