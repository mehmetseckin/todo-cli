<p align="center">
    <img 
        src="./assets/logo.png"
        width="200"
        height="200"
    />
</p>

# MSToDo Tool

MSTTool is a command-line interface to access the Microsoft To Do REST API, using C# and .NET Core 3. Includes an "export" function for exporting all your tasks to JSON files organized in a folder hierarchy to enable automated backups.

## Background

The impetus for creating this tool are limitations with Microsoft To Do, which is an excellent cloud-based task management tool that supports easy management of repeating tasks, due dates, and daily lists. It evolved out of Wunderlist and has come a long way since then. Some of the limitations this tool seeks to address:
* Backup: There is no easy way to backup or export your Tasks. Outlook can interface with your Tasks, and can Export, but only through cumbersome manual steps to a broken CSV format or to an unfriendly PST format.
* Accidental Delete: It is easy to accidentally delete a task, with limited Undo functionality and no history. This can be frustrating if you accidentally swiped left on a task and missed the Undo prompt. (Though I believe Outlook allows you to see and recover Deleted tasks.)
* Accidental Complete: Similarly, it is easy to accidentally "complete" a task. While you can easily "uncomplete" a task, depending on the context you were in, you may not know which task it is.

This particular project is forked from [todo-cli](https://github.com/mehmetseckin/todo-cli). This project is a refactor that kept some of the top-level structure.

## Objectives

The objectives of MSTTool includes:
* Enable automated backups of your Tasks
* Enable "diffing" two snapshots of Tasks *[Not Implemented]*
* Enable more flexible search functions (by virtue of it all being stored as text)

## Getting Started

### Build

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

The application will automatically prompt you to sign in with your Microsoft account, and ask for your consent to access your data when needed.

```
# Run from build output
cd .\MSTTool\bin\Debug\netcoreapp3.1\
todo --help

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

Interested? You are awesome. Feel free to fork, do your thing and send a PR! Everything is appreciated.

## Code of Conduct

Be nice to people, give constructive feedback, and have fun!

## Stack

This project is built using the following nuggets of awesomeness, and many more. Many thanks to the folks who are working on and maintaining these products.

- [.NET Core 3](https://github.com/dotnet/core)
- [System.CommandLine](https://github.com/dotnet/command-line-api)
- [Microsoft Graph Beta SDK](https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet)
- [Inquirer.cs](https://github.com/agolaszewski/Inquirer.cs)
