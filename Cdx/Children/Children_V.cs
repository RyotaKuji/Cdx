using Spectre.Console;
using Spectre.Console.Rendering;

internal class Children_V(Children model)
{
	public IRenderable GetComp(int columnsCount, int height, int page)
	{
		Grid grid = new Grid()
			.AddColumns(columnsCount);

		List<FileSystemInfo> values = model.FilteredValue.ToList();

		int itemsCountPerPage = columnsCount * height;
		int startIndex = itemsCountPerPage * page;

		for (int i = 0; i < height; i++)
		{
			List<FileSystemInfo> row = [];
			for (int j = 0; j < columnsCount; j++)
			{
				int index = startIndex + height * j + i;
				if (index < values.Count)
				{
					row.Add(values[index]);
				}
			}
			grid.AddRow(row.Select(GetItemText).ToArray());
		}

		return grid;
	}

	static string GetItemText(FileSystemInfo item)
	{
		string marker = item is DirectoryInfo ? "📁" : "📄";
		return $"{marker}　{item.Name}";
	}
}
