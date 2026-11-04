internal class Input(Input_V view)
{
	public string ReadLine(InputMode mode)
	{
		string input = view.ReadLine(inputMode_prompt[mode]) ?? string.Empty;
		string formattedText = new(input.Where(c => !char.IsControl(c)).ToArray());
		return formattedText;
	}

	public string ReadLine(ConsoleKeyInfo initialKey, InputMode mode)
	{
		List<ConsoleKeyInfo> sendKeys = [initialKey];
		while (Console.KeyAvailable)
		{
			ConsoleKeyInfo additionalKey = Console.ReadKey(true);
			sendKeys.Add(additionalKey);
		}
		string formattedText = new(sendKeys.Select(k => k.KeyChar).Where(c => !char.IsControl(c)).ToArray());
		view.AddBufferText(formattedText);
		return ReadLine(mode);
	}

	public ConsoleKeyInfo ReadKey(InputMode mode)
	{
		Console.CursorVisible = false;
		ConsoleKeyInfo key = view.ReadKey(inputMode_prompt[mode]);
		return key;
	}

	public enum InputMode
	{
		Input,
		Command,
		FileChange,
	}

	Dictionary<InputMode, string> inputMode_prompt = new()
	{
		[InputMode.Input] = "> ",
		[InputMode.Command] = "[CMD]",
		[InputMode.FileChange] = "# ",
	};
}
