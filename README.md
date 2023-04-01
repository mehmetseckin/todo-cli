<p align="center">
    <img 
        src="./assets/logo.png"
        width="200"
        height="200"
    />
</p>

# MST Tool

MSTTool is a command-line interface to access Microsoft To Do via the Graph API, using C# and .NET Core. Includes:
* an "export" function for exporting all your tasks to JSON files organized in a folder hierarchy to enable automated backups and text search.
* a framework for making other API calls
* *[Not Implemented]* a "sync" function to export and remove deleted tasks, so you can automate backing up your tasks to a repo or other service
* *[Not Implemented]* a "diff" function to compare two snapshots

## Background

The impetus for creating this tool are limitations with Microsoft To Do, which is an excellent cloud-based task management tool that supports easy management of repeating tasks, due dates, and daily lists. It evolved out of Wunderlist and has come a long way since then. Some of the limitations this tool seeks to address:
* Backup: There is no easy way to backup or export your Tasks. Outlook can interface with your Tasks, and can Export, but only through cumbersome manual steps to a broken CSV format or to an unfriendly PST format.
* Accidental Delete: It is easy to accidentally delete a task, with limited Undo functionality and no history. This can be frustrating if you accidentally swiped left on a task and missed the Undo prompt. (Though I believe Outlook allows you to see and recover Deleted tasks.)
* Accidental Complete: Similarly, it is easy to accidentally "complete" a task. While you can easily "uncomplete" a task, depending on the context you were in, you may not know which task it is.
* Performance: As the size of your Task database grows (including completed Tasks), clients (particularly on some platforms like iOS) suffer significant problems with responsiveness, requiring pruning down the number of Tasks. This requires manually "deleting" old completed tasks (again, Outlook can help, but an automated solution would be better).

This project is forked from [todo-cli](https://github.com/mehmetseckin/todo-cli). It is a refactor that follows the original's top-level configuration, but otherwise uses a different implementation.

## Getting Started

### Graph API ClientId 

While it's not necessary to get a unique one, each application that interfaces with the Graph API needs it's own ClientId. A new application can be registered through the Azure website, where you can also specify the "Scopes" for the application (meaning what it's permitted to do with a user account).  The Client Id is stored in the appsettings.json.

### Build

*NOTE: Building the project has only been tested with Visual Studio 2022.*

```
# Clone the repository
git clone https://github.com/pappde/MsToDoTool.git

# Navigate into the source code folder
cd .\MSTTool\src

# Build the project
dotnet build
```

Alternately, you can use Visual Studio and open the MSTTool.sln.

### Run

*NOTE: Running the application has only been tested on Windows.*

The application will automatically prompt you to sign in with your Microsoft account, and ask for your consent to access your data when needed.

```
# Run from build output
cd .\MSTTool\bin\Debug\netcoreapp3.1\
todo --help

# Update the Clientid if desired
vi appsettings.json

# Export
todo export
todo export -folder=ToFolder
todo export OnlyList -folder=ToFolder
```

### Configuration

* appsettings.json: different parameters can be configured here, such as the default TargetFolder
* Target List: Most commands allow taking a list name as a parameter. If no list name is specified, then it will act on all lists.
* Target Folder: Some commands allow changing the TargetFolder specified in the configuration. "-folder=ToFolder"

## Contributing

You are welcome to contribute.

## Dependencies

This project is built using the following, among others.

- [Microsoft.Extensions.Configuration](https://asp.net)
- [Microsoft.Extensions.DependencyInjection](https://github.com/dotnet/runtime)
- [Microsoft.Graph](https://developer.microsoft.com/graph)
- [Microsoft.Identity.Client](https://go.microsoft.com/fwlink/?linkid=844761)
- [.NET Core 3](https://github.com/dotnet/core)
- [Nuget.Common](https://aka.ms/nugetprj) - for AsyncLazy
- [System.CommandLine](https://github.com/dotnet/command-line-api)

## Backlog

Big Things:
* Sync command: same as export, but delete old tasks that no longer exist
* Diff command: compare two exported folders
* Purge command: use the API to cleanup old "completed" tasks according to some rules. E.g. repeating tasks beyond the last "X".
* Releases: setup github releases
* Cross-platform: test if works on other platforms
* Consider switching to .NET Standard or Framework so we can upgrade the packages.

Little Things:
* the TODO.txt file contains some items
* other items are flagged in code using a "TODO" comment

