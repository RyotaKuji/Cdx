internal class Input(Input_V view)
{
	public string ReadLine()
	{
		string input = view.ReadLine();
		return input;
	}

	public string ReadLine(ConsoleKeyInfo initialKey)
	{
		List<ConsoleKeyInfo> keys = [initialKey];
		while (Console.KeyAvailable)
		{
			ConsoleKeyInfo additionalKey = Console.ReadKey(true);
			keys.Add(additionalKey);
		}
		string sendText = new(keys.Select(k => k.KeyChar).Where(c => !char.IsControl(c)).ToArray());
		view.AddBufferText(sendText);
		return ReadLine();
	}

	public ConsoleKeyInfo ReadKey()
	{
		ConsoleKeyInfo key = view.ReadKey();
		return key;
	}

	public void Render()
	{
		view.Render();
	}
}
