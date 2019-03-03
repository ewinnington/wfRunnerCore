using System;
using System.Collections.Generic;
using System.Text;
using WorkflowExtensionInterfaces;

namespace wfRunnerCore.Extensions
{
    class EmailSetting : IEmailSetting
    {
        public string GetAddress()
        {
            try
            {
                return Environment.GetEnvironmentVariable("Mail_ServerEmailAddress");
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string GetHost()
        {
            try
            {
                return Environment.GetEnvironmentVariable("Mail_MailServerAddress");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetPassword()
        {
            try
            {
                return Environment.GetEnvironmentVariable("Mail_ServerEmailPassword");
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string GetUsername()
        {
            try
            {
                return Environment.GetEnvironmentVariable("Mail_ServerEmailAccount");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
