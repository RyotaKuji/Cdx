using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

ServiceCollection services = new();

services.AddSingleton<App>();
services.AddSingleton<Input>();
services.AddSingleton<Input_V>();
services.AddSingleton<CurrentDirectory>();
services.AddSingleton<CurrentDirectory_V>();
services.AddSingleton<SearchTerms>();
services.AddSingleton<SearchTerms_V>();
services.AddSingleton<Children>();
services.AddSingleton<Children_V>();
services.AddSingleton<Message>();
services.AddSingleton<Message_V>();
services.AddSingleton<History>();

using ServiceProvider provider = services.BuildServiceProvider();

App app = provider.GetRequiredService<App>();

// Define command-line options
Option<DirectoryInfo> currentDirectoryOption =
	new("--current-directory")
	{
		Description = "Set the current directory",
		Required = false,
	};

Option<FileInfo> writeResultOption =
	new("--write-result")
	{
		Description = "Write the result to a file",
		Required = false,
	};

RootCommand rootCommand = new("Cdx");

rootCommand.Options.Add(currentDirectoryOption);
rootCommand.Options.Add(writeResultOption);
rootCommand.SetAction(parseResult =>
{
	DirectoryInfo currentDirectory =
		parseResult.GetValue(currentDirectoryOption) ?? new(Path.GetPathRoot(Environment.CurrentDirectory) ?? throw new InvalidOperationException("Cannot determine the root directory."));

	FileInfo? outFile =
		parseResult.GetValue(writeResultOption);

	app.Run(currentDirectory, outFile);
});

return rootCommand.Parse(args).Invoke();
