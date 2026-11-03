internal class Input(Input_V view)
{
	public string ReadLine()
	{
		string input = view.ReadLine() ?? string.Empty;
		string formattedText = new(input.Where(c => !char.IsControl(c)).ToArray());
		return formattedText;
	}

	public string ReadLine(ConsoleKeyInfo initialKey)
	{
		List<ConsoleKeyInfo> sendKeys = [initialKey];
		while (Console.KeyAvailable)
		{
			ConsoleKeyInfo additionalKey = Console.ReadKey(true);
			sendKeys.Add(additionalKey);
		}
		string formattedText = new(sendKeys.Select(k => k.KeyChar).Where(c => !char.IsControl(c)).ToArray());
		view.AddBufferText(formattedText);
		return ReadLine();
	}

	public ConsoleKeyInfo ReadKey()
	{
		Console.CursorVisible = false;
		ConsoleKeyInfo key = view.ReadKey();
		return key;
	}
}
