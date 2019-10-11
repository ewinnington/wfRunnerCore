using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using wfRunnerCore.Extensions;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Activities.Tracking;
using System.Xaml;

namespace wfRunnerCore
{
    class WorkflowRunnerCore
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += FindAssem;

            if (args.Length > 1)
            {
                try
                {
                    if (args[0].Equals("-h"))
                    {
                        string message = "Usage: -r WorkflowFileName.Ext -a ArgumentFile.json \n"+
                                         "       -r \"Path\\WorkflowFileName.Ext\"  -a \"Path\\ArgumentFile.json\" \n" +
                                         "       -r \"Path\\WorkflowFileName.Ext\"  -x \"Path\\ArgumentFile.xaml\"";
                        Console.WriteLine(message); Console.ReadKey();
                        return;
                    }

                    if (args[0].Equals("-r"))
                    {
                        //run workflow
                        int idx = Array.IndexOf(args, "-r");
                        string FileName = args[idx + 1];
                        string wfText = File.ReadAllText(FileName);

                        System.IO.StringReader stringReader = new System.IO.StringReader(wfText);

                        var settings = new ActivityXamlServicesSettings { CompileExpressions = true };
                        Activity root = System.Activities.XamlIntegration.ActivityXamlServices.Load(stringReader, settings );
                        
                        Dictionary<string, object> WorkflowArguments = new Dictionary<string, object>();

                        idx = Array.IndexOf(args, "-a"); //json arguments
                        if (idx != -1)
                        {
                            string ArgumentFile = args[idx + 1];
                            WorkflowArguments = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(ArgumentFile));
                        }

                        idx = Array.IndexOf(args, "-x"); //xaml based argument serialization - allows for complex objects
                        if(idx != -1)
                        {
                           WorkflowArguments = (Dictionary<string, object>)XamlServices.Load(args[idx + 1]);
                        }

                        WorkflowApplication wfApp = new WorkflowApplication(root, WorkflowArguments);

                        WFTracking track = new WFTracking(new ConsoleLogger());
                        //Tracking Profile
                        TrackingProfile prof = new TrackingProfile();
                        prof.Name = "CustomTrackingProfile";
                        prof.Queries.Add(new WorkflowInstanceQuery { States = { "*" } });
                        prof.Queries.Add(new ActivityStateQuery { States = { "*" }, Arguments = { "*" } });
                        prof.Queries.Add(new CustomTrackingQuery() { Name = "*", ActivityName = "*" });
                        prof.ImplementationVisibility = ImplementationVisibility.RootScope;
                        track.TrackingProfile = prof;
                        track.WorkflowName = Path.GetFileNameWithoutExtension(FileName);

                        wfApp.Extensions.Add(track);

                        wfApp.Extensions.Add(new EmailSetting());
                        //Requires the following environment variables to be set: Mail_MailServerAddress, Mail_ServerEmailAddress, Mail_ServerEmailPassword, Mail_ServerEmailAccount, 

                        wfApp.Extensions.Add(new ConsoleLogger());

                        wfApp.Completed = (WorkflowApplicationCompletedEventArgs e) => {
                            if (e.Outputs != null && e.Outputs.Count > 0) {
                                foreach (var kvp in e.Outputs)
                                    Console.WriteLine("Output --> {0} : {1}", kvp.Key, kvp.Value.ToString());
                            }
                            Console.WriteLine("Finished - Workflow Completion State {0} \nPress any key to continue", e.CompletionState); };
                        wfApp.Run();
                    }
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not open workflow: " + string.Join(" ", args) + "\nError" + e.ToString());
                }
                Console.WriteLine("Waiting... press key to exit");
                Console.ReadKey();
            }
        }

        private static System.Reflection.Assembly FindAssem(object sender, ResolveEventArgs args)
        {
            ///Check if Assembly is already loaded
            Assembly[] asmList = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in asmList)
            {
                if (item.FullName.Equals(args.Name))
                    return item;
            }

            string AssemblyPartialName = args.Name;

            if (AssemblyPartialName.Contains(','))
            {
                AssemblyPartialName = AssemblyPartialName.Split(',')[0];
            }
            string CurrentPath = AppContext.BaseDirectory;
            string ExpectedDllPath = Path.Combine(CurrentPath, @"WFLibs\" + AssemblyPartialName + ".dll"); 
            ///Look for Assembly in the predefined folder
            if (File.Exists(ExpectedDllPath))
            {
                var path = ExpectedDllPath;
                //Normal assembly load from file path
                return Assembly.LoadFrom(path);
            }

            if (false) { 
                //simulate loading a binary from elsewhere (ie. DB storage)
                byte[] BinAssembly = File.ReadAllBytes(ExpectedDllPath);
                return Assembly.Load(BinAssembly);
            }

            return null;
        }
    }
}
