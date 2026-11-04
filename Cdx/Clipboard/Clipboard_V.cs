using Spectre.Console;
using Spectre.Console.Rendering;

internal class Clipboard_V(Clipboard model)
{
	const int maxChildrenCount = 4;
	public IRenderable GetComp(int width)
	{
		Grid grid = new Grid().AddColumns(2);
		for (int i = 0; i < model.Values.Count; i++)
		{
			IEnumerable<FileSystemInfo> items = model.Values[i];
			int displayedCount = 0;
			foreach (FileSystemInfo item in items)
			{
				Text indexText = displayedCount == 0 ? new($"${i}") : Text.Empty;
				grid.AddRow(indexText, new TextPath(item.FullName));
				displayedCount++;
				if (displayedCount > maxChildrenCount)
				{
					int remainingCount = items.Count() - displayedCount;
					if (remainingCount > 0)
					{
						grid.AddRow(Text.Empty, new Text($" ⋮ +{remainingCount}"));
					}
					break;
				}
			}
			grid.AddEmptyRow();
		}
		return grid;
	}

	public bool HasValue => model.Values.Count > 0;
}
