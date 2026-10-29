internal class Input_V
{
	public string ReadLine()
	{
		return Console.ReadLine() ?? string.Empty;
	}

	public ConsoleKeyInfo ReadKey()
	{
		return Console.ReadKey();
	}
}
