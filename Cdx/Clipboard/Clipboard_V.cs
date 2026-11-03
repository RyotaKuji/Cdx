using Spectre.Console;
using Spectre.Console.Rendering;

internal class Clipboard_V(Clipboard model)
{
	const int maxChildrenCount = 4;
	public IRenderable GetComp(int width)
	{
		int index = 0;
		Grid grid = new Grid().AddColumns(2);
		foreach (var parent_children in model.Values)
		{
			string parent = parent_children.Key;
			IEnumerable<FileSystemInfo> children = parent_children.Value;
			grid.AddRow(new Text($"${index}"), new TextPath(parent));
			grid.AddEmptyRow();
			index++;
			if (children.Any())
			{
				int currentChildrenCount = 0;
				foreach (FileSystemInfo child in children)
				{
					string childIndexText = currentChildrenCount == 0 ? $"${index}" : string.Empty;
					grid.AddRow(new Text(childIndexText), new Text(child.Name));

					currentChildrenCount++;

					if (currentChildrenCount >= maxChildrenCount)
					{
						int remainingChildCount = children.Count() - currentChildrenCount;
						if (remainingChildCount > 0)
						{
							grid.AddRow(Text.Empty, new Text($" ⋮ +{remainingChildCount}"));
						}
						break;
					}
				}
				index++;
			}
			grid.AddEmptyRow();
		}
		return grid;
	}

	public bool HasValue => model.Values.Count > 0;
}
