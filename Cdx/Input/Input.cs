internal class Input(Input_V view)
{
	public string ReadLine()
	{
		view.Render(mode_prompt[InputMode.Text]);
		string input = view.ReadLine();
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
		view.Render(mode_prompt[InputMode.Command]);
		Console.CursorVisible = false;
		ConsoleKeyInfo key = view.ReadKey();
		return key;
	}

	enum InputMode
	{
		Text,
		Command,
	}

	static Dictionary<InputMode, string> mode_prompt = new()
	{
		{ InputMode.Text, "> " },
		{ InputMode.Command, "[COMMAND]" },
	};
}
