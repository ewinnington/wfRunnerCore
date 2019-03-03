using System;

namespace WorkflowExtensionInterfaces
{
    public interface IEmailSetting
    {
        string GetHost();
        string GetUsername();
        string GetPassword();
        string GetAddress();
    }

}
