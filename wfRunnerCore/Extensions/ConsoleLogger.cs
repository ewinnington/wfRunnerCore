using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WorkflowExtensionInterfaces;

namespace wfRunnerCore.Extensions
{
    public class ConsoleLogger : ILogWorkflowEvents
    {
        public void InsertEvent(WorkflowRecord EventRecord)
        {
            Console.WriteLine(EventRecord.ToString('\t'));
        }

        public void UpdateEvent(WorkflowRecord EventRecord)
        {
            Console.WriteLine(EventRecord.ToString('\t').TrimEnd('\n').Split('\n').Last());
        }

        public void Dispose()
        {
        }
    }
}
