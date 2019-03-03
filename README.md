# wfRunnerCore
Building a minimum CoreWf workflow runner that supports dynamic loading of dlls with activities. Includes a basic set of Extensions (Logger, Email Server address) and an activity DLL with Email, Zip and RunCommand demonstration. Uses [dmetzgar/CoreWf](https://github.com/dmetzgar/corewf) as a submodule. 

This runner is developed to test the features of [CoreWf](https://github.com/dmetzgar/corewf) and discover the differences in behaviors between Workflow-Foundation on the .Net Framework and CoreWf on .net core.

Please ensure that the "ActivityLibraryCore.dll" is located in the WFLibs directory. This folder is where the wfRunnerCore looks for activity code if they are not already loaded in memory. 

Usage on the command line: 

dotnet wfRunnerCore.dll -r "Workflows\RunMe.xaml"
dotnet wfRunnerCore.dll -r "Workflows\EchoWorkflowArgument.xaml" -a "Workflows\EchoWorkflowArgument.json"
dotnet wfRunnerCore.dll -r "Workflows\IncrementInputToOutput.xaml" -a "Workflows\IncrementInputToOutput.json"
