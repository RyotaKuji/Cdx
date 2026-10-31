using System;
using System.Collections.Generic;
using System.Text;

internal static class Result
{
	public static void Return(string result, FileInfo? resultFile)
	{
		Console.Clear();
		try
		{
			if (resultFile is not null)
			{
				File.WriteAllText(resultFile.FullName, result);
			}
			else
			{
				Console.WriteLine(result);
			}
			Environment.Exit(0);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Error writing result to '{resultFile?.FullName}': {ex.Message}");
			Environment.Exit(1);
		}
	}
}
