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
		var map = Parser.GetMap();

		var strategy = new DetourStrategy(map);
		strategy.Process();

		for (int i = 0; i < map.Height; i++)
		{
			var skip = i * map.Width;
			var cellsRow = map.Cells
				.Skip(skip)
				.Take(map.Width)
				.Select(x => x.NumberOfVisits)
				.Select(x => x == -1 ? "#" : x.ToString())
				.Aggregate((x, y) => $"{x}{y}");

			Console.WriteLine(cellsRow);
		}
	}
}

class Map
{
	public Map(int width, int height, IEnumerable<Cell> cells, Cell startCell, string initialDirection, string side)
	{
		this.Width = width;
		this.Height = height;
		this.Cells = cells;
		this.Start = cells.First(x => x == startCell);

		this._cellsDict = cells.ToDictionary(x => $"{x.X}_{x.Y}", x => x);

		switch (initialDirection)
		{
			case ">":
				this.Orientation = Direction.Right;
				break;
			case "v":
				this.Orientation = Direction.Down;
				break;
			case "<":
				this.Orientation = Direction.Left;
				break;
			case "^":
				this.Orientation = Direction.Top;
				break;
		}

		switch (side)
		{
			case "R":
				this.Side = Direction.Right;
				break;
			case "L":
				this.Side = Direction.Left;
				break;
		}
	}

	public IEnumerable<Cell> Cells { get; }
	public int Height { get; }
	public int Width { get; }
	public Cell Start { get; set; }
	public Direction Orientation { get; }
	public Direction Side { get; }

	private Dictionary<string, Cell> _cellsDict;

	public IEnumerable<Cell> GetNeighborCells(Cell cell)
	{
		var topCell = this.OnPosition(cell.X - 1, cell.Y);
		var rightCell = this.OnPosition(cell.X, cell.Y + 1);
		var bottomCell = this.OnPosition(cell.X + 1, cell.Y);
		var leftCell = this.OnPosition(cell.X, cell.Y - 1);

		if (rightCell == Cell.NullCell)
		{
			rightCell = this.OnPosition(cell.X, 0);
		}

		if (leftCell == Cell.NullCell)
		{
			leftCell = this.OnPosition(cell.X, this.Width - 1);
		}

		if (topCell == Cell.NullCell)
		{
			var newY = cell.Y - (this.Width / 2);
			if (newY < 0)
			{
				newY = this.Width + newY;
			}

			topCell = this.OnPosition(this.Height - 1, newY);
		}

		if (bottomCell == Cell.NullCell)
		{
			var newY = cell.Y - (this.Width / 2);
			if (newY < 0)
			{
				newY = this.Width + newY;
			}

			bottomCell = this.OnPosition(0, newY);
		}

		return (new List<Cell>
		{
			topCell,
			rightCell,
			bottomCell,
			leftCell
		});
	}

	private Cell OnPosition(int x, int y)
	{
		if (x < 0 || y < 0 || x >= this.Height || y >= this.Width)
		{
			return Cell.NullCell;
		}

		return this._cellsDict[$"{x}_{y}"];
	}
}

class DetourStrategy
{
	public Map Map { get; set; }
	private Direction _currentOrientation;

	public DetourStrategy(Map map)
	{
		this.Map = map;
		this._currentOrientation = this.Map.Orientation;
	}

	public void Process()
	{
		var initialCell = this.Map.Start;
		initialCell.Visit();
		var currentCell = this.VisitNextCell(initialCell);
		while (currentCell != initialCell)
		{
			currentCell.Visit();
			currentCell = this.VisitNextCell(currentCell);
		}
	}

	private Cell VisitNextCell(Cell currentCell)
	{
		var cells = this.Map.GetNeighborCells(currentCell).ToArray();
		var orderedCells = this.GetNeighborCells(cells, this._currentOrientation, this.Map.Side);

		var nextCell = orderedCells.FirstOrDefault(x => x.IsPassable);

		if (nextCell == null)
		{
			return currentCell;
		}

		this._currentOrientation = GetNewOrientation(Array.IndexOf(cells, nextCell));

		return nextCell;
	}

	private Direction GetNewOrientation(int nextCellIndex)
	{
		if (nextCellIndex == 0)
		{
			return Direction.Top;
		}
		if (nextCellIndex == 1)
		{
			return Direction.Right;
		}
		if (nextCellIndex == 2)
		{
			return Direction.Down;
		}

		return Direction.Left;
	}

	private Cell[] GetNeighborCells(Cell[] cells, Direction currentOrientation, Direction side)
	{
		//LEFT
		if (currentOrientation == Direction.Right && side == Direction.Left)
		{
			return cells.ToArray();
		}

		if (currentOrientation == Direction.Down && side == Direction.Left)
		{
			return new Cell[]
			{
				cells[1],
				cells[2],
				cells[3],
				cells[0]
			};
		}

		if (currentOrientation == Direction.Left && side == Direction.Left)
		{
			return new Cell[]
			{
				cells[2],
				cells[3],
				cells[0],
				cells[1]
			};
		}

		if (currentOrientation == Direction.Top && side == Direction.Left)
		{
			return new Cell[]
			{
				cells[3],
				cells[0],
				cells[1],
				cells[2]
			};
		}
		//RIGHT
		if (currentOrientation == Direction.Right && side == Direction.Right)
		{
			return new Cell[]
			{
				cells[2],
				cells[1],
				cells[0],
				cells[3]
			};
		}

		if (currentOrientation == Direction.Down && side == Direction.Right)
		{
			return new Cell[]
			{
				cells[3],
				cells[2],
				cells[1],
				cells[0]
			};
		}

		if (currentOrientation == Direction.Left && side == Direction.Right)
		{
			return new Cell[]
			{
				cells[0],
				cells[3],
				cells[2],
				cells[1]
			};
		}

		if (currentOrientation == Direction.Top && side == Direction.Right)
		{
			return new Cell[]
			{
				cells[1],
				cells[0],
				cells[3],
				cells[2]
			};
		}

		throw new ArgumentException();
	}
}

class Cell : IEquatable<Cell>
{
	public Cell(int x, int y, bool isPassable)
	{
		this.X = x;
		this.Y = y;
		this.IsPassable = isPassable;

		if (!isPassable)
		{
			this.NumberOfVisits = -1;
		}
	}

	public int X { get; }
	public int Y { get; }
	public bool IsPassable { get; }
	public int NumberOfVisits { get; private set; }

	public void Visit()
	{
		this.NumberOfVisits++;
	}

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

enum Direction
{
	Right,
	Down,
	Left,
	Top
}

static class Parser
{
	public static Map GetMap()
	{
		string[] inputs = Console.ReadLine().Split(' ');
		int width = int.Parse(inputs[0]);
		int height = int.Parse(inputs[1]);

		var cells = new List<Cell>();
		var initialDirection = string.Empty;
		var startCell = Cell.NullCell;

		var directions = new List<char>()
		{
			'<', '>', 'v', '^'
		};

		for (int i = 0; i < height; i++)
		{
			string line = Console.ReadLine();

			for (int j = 0; j < width; j++)
			{
				var cell = line[j];

				if (directions.Contains(cell))
				{
					initialDirection = cell.ToString();
					startCell = new Cell(i, j, true);
					cells.Add(startCell);
				}
				else
				{
					var cellIsPassable = cell == '0';

					cells.Add(new Cell(i, j, cellIsPassable));
				}
			}
		}
		string side = Console.ReadLine();

		return new Map(width, height, cells, startCell, initialDirection, side);
	}
}