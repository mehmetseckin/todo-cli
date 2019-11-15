<p align="center">
    <img 
        src="./assets/logo.png"
        width="200"
        height="200"
    />
</p>

# Todo CLI

A cross-platform command-line interface to interact with Microsoft To Do, built using .NET Core 3.

## Getting Started

### Build

```
# Clone the repository
git clone https://github.com/mehmetseckin/todo-cli.git

# Navigate into the source code folder
cd .\todo-cli\src

# Build the project
dotnet build
```

### Run

The application will automatically prompt you to sign in with your Microsoft account, and ask for your consent to access your data when needed.

```
# Run using dotnet run
dotnet run -p .\Todo.CLI -- --help

# Run from build output (?)
.\Todo.CLI\bin\Debug\netcoreapp3.0\todo --help
```

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
