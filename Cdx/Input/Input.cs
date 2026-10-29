internal class Input(Input_V view)
{
	public string ReadLine()
	{
		string input = view.ReadLine();
		return input;
	}

	public ConsoleKeyInfo ReadKey()
	{
		ConsoleKeyInfo key = view.ReadKey();
		return key;
	}
}
