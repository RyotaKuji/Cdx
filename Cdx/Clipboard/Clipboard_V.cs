using Spectre.Console;
using Spectre.Console.Rendering;

internal class Clipboard_V(Clipboard model)
{
	const int maxChildrenCount = 4;
	public IRenderable GetComp(int height)
	{
		Grid grid = new Grid().AddColumns(2);
		for (int i = 0; i < model.Values.Count; i++)
		{
			Clipboard.ClipboardItemType type = model.Values[i].Type;
			IEnumerable<FileSystemInfo> items = model.Values[i].Info;
			int displayedCount = 0;
			foreach (FileSystemInfo item in items)
			{
				Text indexText = displayedCount == 0 ? new($"<{i}>") : Text.Empty;

				string path = type == Clipboard.ClipboardItemType.Parent ?
					item.FullName :
					item.Name;

				IRenderable pathComp;
				if (type == Clipboard.ClipboardItemType.Parent)
				{
					pathComp = new Text(item.FullName);
				}
				else
				{
					pathComp = new Panel(new Text(item.Name))
					{
						Padding = new(2, 0, 0, 0),
						Border = BoxBorder.None,
					};
				}

				grid.AddRow(indexText, pathComp);
				displayedCount++;
				if (displayedCount > maxChildrenCount)
				{
					int remainingCount = items.Count() - displayedCount;
					if (remainingCount > 0)
					{
						grid.AddRow(Text.Empty, new Text($"   ⋮ +{remainingCount}"));
					}
					break;
				}
			}
			grid.AddEmptyRow();
		}
		Panel panel = new(grid)
		{
			Border = BoxBorder.Rounded,
			Height = height,
			Expand = true,
		};
		return panel;
	}

	public bool HasValue => model.Values.Count > 0;
}
