# wfRunnerCore
Building a minimum CoreWf workflow runner that supports dynamic loading of dlls with activities. Includes a basic set of Extensions (Logger, Email Server address) and an activity DLL with Email, Zip and RunCommand demonstration. Uses [dmetzgar/CoreWf](https://github.com/dmetzgar/corewf) as a submodule. 

This runner is developed to test the features of [CoreWf](https://github.com/dmetzgar/corewf) and discover the differences in behaviors between Workflow-Foundation on the .Net Framework and CoreWf on .net core.

Please ensure that the "ActivityLibraryCore.dll" is located in the WFLibs directory. This folder is where the wfRunnerCore looks for activity code if they are not already loaded in memory. 

Usage on the command line: 

dotnet wfRunnerCore.dll -r "Workflows\RunMe.xaml"

dotnet wfRunnerCore.dll -r "Workflows\EchoWorkflowArgument.xaml" -a "Workflows\EchoWorkflowArgument.json"

dotnet wfRunnerCore.dll -r "Workflows\IncrementInputToOutput.xaml" -a "Workflows\IncrementInputToOutput.json"

dotnet wfRunnerCore.dll -r "Workflows\worflowFile.xaml" -x "Workflows\worklowFileArgs.xaml"

Please note that the Arguments are parsed from a json file with by Newtonsoft Json.net with the "-a" command. Therefore, the InArguments should be adapted to the default data type. For example, an integer is parsed as a long (Int64) as an argument. With the "-x" command, the arguments are loaded from a XAML serialization, so you can pass in complex objects, as long as your argument file defines a Dictionary<string,object>, please see [DoodleLemonArgs.xaml](https://github.com/ewinnington/wfRunnerCore/blob/master/wfRunnerCore/Workflows/DoodleLemonArgs.xaml) for an example.

License: MIT
