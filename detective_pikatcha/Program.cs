using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
	static void Main(string[] args)
	{
		string[] inputs = Console.ReadLine().Split(' ');
		int width = int.Parse(inputs[0]);
		int height = int.Parse(inputs[1]);

		List<Cell> cells = new List<Cell>();

		for (int i = 0; i < height; i++)
		{
			string line = Console.ReadLine();
			for (int j = 0; j < width; j++)
			{
				var cell = line[j];
				cells.Add(new Cell(i, j, cell == '0'));
			}
		}

		var map = new Map(width, height, cells);

		for (int i = 0; i < height; i++)
		{

			var skip = i * width;
			var cellsRow = map.Cells
				.Skip(skip)
				.Take(width)
				.Select(x => map.GetNumberOfNeighborCells(x))
				.Select(x => x == -1 ? "#" : x.ToString())
				.Aggregate((x, y) => $"{x}{y}");

			Console.WriteLine(cellsRow);
		}
	}
}

class Map
{
	public Map(int width, int height, IEnumerable<Cell> cells)
	{
		this.Width = width;
		this.Height = height;
		this.Cells = cells;
	}

	public IEnumerable<Cell> Cells { get; }
	public int Height { get; }
	public int Width { get; }

	public IEnumerable<Cell> GetNeighborCells(Cell cell)
	{
		var topCell = new Cell(cell.X - 1, cell.Y, this.OnPosition(cell.X - 1, cell.Y).IsPassable);
		var rightCell = new Cell(cell.X, cell.Y + 1, this.OnPosition(cell.X, cell.Y + 1).IsPassable);
		var bottomCell = new Cell(cell.X + 1, cell.Y, this.OnPosition(cell.X + 1, cell.Y).IsPassable);
		var leftCell = new Cell(cell.X, cell.Y - 1, this.OnPosition(cell.X, cell.Y - 1).IsPassable);

		return (new List<Cell>
		{
			topCell,
			rightCell,
			bottomCell,
			leftCell
		}).Where(x => this.Cells.Contains(x));
	}

	public int GetNumberOfNeighborCells(Cell cell)
	{
		if (!cell.IsPassable)
		{
			return -1;
		}

		return this.GetNeighborCells(cell).Count(x => x.IsPassable);
	}

	private Cell OnPosition(int x, int y)
	{
		return this.Cells.FirstOrDefault(c => c.X == x && c.Y == y) ?? Cell.NullCell;
	}
}

class Cell : IEquatable<Cell>
{
	public Cell(int x, int y, bool isPassable)
	{
		this.X = x;
		this.Y = y;
		this.IsPassable = isPassable;
	}

	public int X { get; }
	public int Y { get; }
	public bool IsPassable { get; }

	public static Cell NullCell => new Cell(-1, -1, false);

	public override bool Equals(object obj)
	{
		return this.Equals(obj as Cell);
	}

	public bool Equals(Cell other)
	{
		return other != null &&
			   this.X == other.X &&
			   this.Y == other.Y;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(this.X, this.Y);
	}

	public static bool operator ==(Cell cell1, Cell cell2)
	{
		return EqualityComparer<Cell>.Default.Equals(cell1, cell2);
	}

	public static bool operator !=(Cell cell1, Cell cell2)
	{
		return !(cell1 == cell2);
	}
}