using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

internal static partial class CommandManager
{
	static readonly Regex clipboardIndexPattern = ReplacedPattern();
	public static (string output, string error) Execute(string command, CurrentDirectory currentDirectory, Clipboard clipboard)
	{
		string[] commandParts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		StringBuilder formattedCommand = new();

		for (int i = 0; i < commandParts.Length; i++)
		{
			string part = commandParts[i];
			string formattedPart = part;
			if (clipboardIndexPattern.IsMatch(part))
			{
				Match match = clipboardIndexPattern.Match(part);
				int index = int.Parse(match.Groups["index"].Value);
				string prefix = match.Groups["prefix"].Value;
				string suffix = match.Groups["suffix"].Value;

				if (clipboard.Values.Count > index)
				{
					IEnumerable<FileSystemInfo> clipboardItems = clipboard.Values[index];
					List<string> formattedFilePaths = [];
					foreach (FileSystemInfo clipboardItem in clipboardItems)
					{
						string formattedFilePath = string.Empty;
						if (clipboardItem is DirectoryInfo directory)
						{
							formattedFilePath = Path.Combine(
								directory.Parent?.FullName ?? string.Empty,
								$"{prefix}{directory.Name}{suffix}"
							);
						}
						else if (clipboardItem is FileInfo file)
						{
							formattedFilePath = Path.Combine(
								file.DirectoryName ?? string.Empty,
								$"{prefix}{Path.GetFileNameWithoutExtension(file.Name)}{suffix}{Path.GetExtension(file.Name)}"
							);
						}
						formattedFilePaths.Add($"\"{formattedFilePath}\"");
					}
					formattedPart = string.Join(",", formattedFilePaths);
				}
			}
			formattedCommand.Append($" {formattedPart}");
		}

		try
		{
			ProcessStartInfo startInfo = new()
			{
				FileName = "pwsh",
				WorkingDirectory = currentDirectory.Value.FullName,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};
			startInfo.ArgumentList.Add("-Command");
			startInfo.ArgumentList.Add("-NonInteractive");
			startInfo.ArgumentList.Add(
				$"""
				[Console]::OutputEncoding = [Text.Encoding]::UTF8;
				$PSStyle.OutputRendering = 'PlainText';
				$ErrorView = 'NormalView';
				{formattedCommand}
				""");
			using Process process = new() { StartInfo = startInfo };
			process.Start();
			string output = process.StandardOutput.ReadToEnd();
			string error = process.StandardError.ReadToEnd();
			process.WaitForExit();

			return (output, error);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while executing the command: {ex.Message}");
			return (string.Empty, ex.Message);
		}
	}

	[GeneratedRegex(@"(?<prefix>.*)<(?<index>\d+)>(?<suffix>.*)")]
	private static partial Regex ReplacedPattern();
}
