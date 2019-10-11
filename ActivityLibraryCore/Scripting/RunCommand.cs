using System.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Activities.Tracking;

namespace ActivityLibraryCore.Scripting
{
    public sealed class RunCommand : CodeActivity<bool>
    {
        public InArgument<string> Command { get; set; }
        public InArgument<string> Arguments { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            string p = Command.Get(context);
            string s = Arguments.Get(context);

            Process RunProc = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(p, s);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            RunProc.StartInfo = psi;
            RunProc.Start();
            string output = RunProc.StandardOutput.ReadToEnd();
            string error = RunProc.StandardError.ReadToEnd();
            RunProc.WaitForExit();

            var record = new CustomTrackingRecord("Info");
            record.Data.Add(new KeyValuePair<string, object>("Message", output + "\n" + error));
            context.Track(record);

            int exitCode = RunProc.ExitCode;
            if (exitCode > 0)
            {
                record = new CustomTrackingRecord("Error");
                record.Data.Add(new KeyValuePair<string, object>("Message", "Command exited with Error Code = " + exitCode));
                context.Track(record);
                return false;
            }
            return true;
        }
    }
}
