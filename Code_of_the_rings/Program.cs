using System;
using System.Collections.Generic;
using System.Linq;

namespace Code_of_the_rings
{
	//https://www.codingame.com/ide/puzzle/code-of-the-rings
	class Program
	{
		private static int[] _phraze;
		private static int[] _zones;

		static void Main()
		{
			var magicPhrase = Console.ReadLine();
			// To debug: Console.Error.WriteLine("Debug messages...");

			_phraze = AlphabetHelper.ToNumberArray(magicPhrase);
			_zones = new int[30];

			var solution = SimpleStrategy.Solve(_zones, _phraze);
			var otherSolution = ClusteredStrategy.Solve(_zones, _phraze);

			Console.WriteLine(solution);
		}
	}

	public class ClusteredStrategy
	{
		private const int MaxNumberOfClusters = 6;

		public static string Solve(int[] zones, int[] phraze)
		{
			var lettersClusters = ClusterizeLetters(phraze);

			return "";
		}

		private static Cluster ClusterizeLetters(int[] phraze)
		{
			var uniqueLetters = phraze.Distinct().ToArray();
			var distances = new List<LetterDistance>();

			foreach (var letter in uniqueLetters)
			{
				foreach (var otherLetter in uniqueLetters)
				{
					distances.Add(new LetterDistance(letter, otherLetter, AlphabetHelper.GetDistanceBetweenLetters(letter, otherLetter)));
				}
			}

			var clusterizer = new Clusterizer(distances, uniqueLetters);
			return clusterizer.Clusterize();
		}
	}

	public class Clusterizer
	{
		private readonly IEnumerable<LetterDistance> _distances;
		private readonly int[] _uniqueLetters;

		public Clusterizer(IEnumerable<LetterDistance> distances, int[] uniqueLetters)
		{
			this._distances = distances.Distinct(new DistanceComparer())
				.OrderByDescending(x => x.Dist).Where(x=>x.Dist != 0);
			this._uniqueLetters = uniqueLetters;
		}

		public Cluster Clusterize()
		{
			var cluster = new Cluster()
			{
				Items = _uniqueLetters,
			};

			var exceptFirst = _distances.Skip(1).ToList();
			while (this.CanClusterize(exceptFirst))
			{
				var leftSublusterMembers = this.GetSubclusterMembers(exceptFirst);
				if (leftSublusterMembers.Count < _uniqueLetters.Length)
				{
					var distancesForLeftSubcluster = _distances
						.Where(x => leftSublusterMembers.Contains(x.A) && leftSublusterMembers.Contains(x.B)).ToList();
					var otherDistances = _distances.Except(distancesForLeftSubcluster);

					cluster.LeftSubcluster = new Clusterizer(distancesForLeftSubcluster, leftSublusterMembers.ToArray()).Clusterize();
					cluster.RightSubcluster = new Clusterizer(otherDistances, _uniqueLetters.Except(leftSublusterMembers).ToArray())
						.Clusterize();

					break;
				}

				exceptFirst = exceptFirst.Skip(1).ToList();
			}

			return cluster;
		}


		private List<int> GetSubclusterMembers(List<LetterDistance> exceptFirst)
		{
			var visitedLetters = new List<int>();
			var lettersToVisit = new Queue<int>();
			lettersToVisit.Enqueue(exceptFirst.First().A);

			while (lettersToVisit.Any())
			{
				var current = lettersToVisit.Dequeue();
				var connectedFromA = exceptFirst.Where(x => x.A == current).Select(x => x.B);
				var connectedToA = exceptFirst.Where(x => x.B == current).Select(x => x.A);
				var connected = connectedToA.Concat(connectedFromA).Distinct().Where(x => !visitedLetters.Contains(x)).ToArray();

				if (!connected.Any())
				{
					continue;
				}

				visitedLetters.AddRange(connected);
				lettersToVisit.Enqueue(connected.First());
			}

			return visitedLetters.Distinct().ToList();
		}

		private bool CanClusterize(IEnumerable<LetterDistance> distances)
		{
			var letterDistances = distances as LetterDistance[] ?? distances.ToArray();
			if (!letterDistances.Any() || letterDistances.Count() < 2)
			{
				return false;
			}

			return true;
		}
	}

	public class Cluster
	{
		public IEnumerable<int> Items { get; set; }
		public int NumberOfItems => Items.Count();
		public Cluster LeftSubcluster { get; set; }
		public Cluster RightSubcluster { get; set; }
	}

	public class DistanceComparer : IEqualityComparer<LetterDistance>
	{
		public bool Equals(LetterDistance x, LetterDistance y)
		{
			if (x.A == y.A && x.B == y.B)
			{
				return true;
			}

			return x.A == y.B && x.B == y.A;
		}

		public int GetHashCode(LetterDistance obj)
		{
			return obj.A * 137 + obj.B * 151 + obj.B * 137 + obj.A * 151; ;
		}
	}

	public struct LetterDistance
	{
		public LetterDistance(int a, int b, int dist)
		{
			this.A = a;
			this.B = b;
			this.Dist = dist;
		}

		public int A { get; }
		public int B { get; }
		public int Dist { get; }
	}

	public class SimpleStrategy
	{
		private readonly BoardState _state;

		private SimpleStrategy(BoardState initialState)
		{
			this._state = initialState;
		}

		public static string Solve(int[] zones, IEnumerable<int> phraze)
		{
			var state = new BoardState(zones, 0);
			var solution = string.Empty;

			foreach (var letter in phraze)
			{
				SimpleStrategy s = new SimpleStrategy(state);
				var nextMove = s.SolveInternal(letter);
				solution += nextMove.path;
				state = nextMove.state;
			}

			return solution;
		}

		private (BoardState state, string path) SolveInternal(int nextLetter)
		{
			var allPosibleStates = _state.State.Select((x, i) => _state.Transform(i, nextLetter)).ToList();

			var bestState = allPosibleStates.GroupBy(x => x.GetDistanceTo(_state)).OrderBy(x => x.Key).First().First();

			var movement = _state.MoveTo(bestState);

			return (bestState, movement);
		}
	}

	public class BoardState
	{
		private const int BoardSize = 30;
		private readonly int _playerPosition;


		public BoardState(int[] state, int playerPosition)
		{
			State = state;
			_playerPosition = playerPosition;
		}

		public int[] State { get; }

		public BoardState Transform(int position, int desiredState)
		{
			if (position >= this.State.Length)
			{
				position = 0;
			}

			if (position < 0)
			{
				position = this.State.Length - 1;
			}

			var stateCopy = (int[])this.State.Clone();
			stateCopy[position] = desiredState;

			return new BoardState(stateCopy, position);
		}

		public string MoveTo(BoardState other)
		{
			var move = string.Empty;

			var distRight = this.GetRightDistance(this._playerPosition, other._playerPosition, BoardSize);
			var distLeft = BoardSize - distRight;

			if (distRight != 0 && distLeft != 0)
			{
				if (distRight < distLeft)
				{
					move += Enumerable.Range(0, distRight).Select(x => ">").Aggregate((x, y) => x + y);
				}
				else
				{
					move += Enumerable.Range(0, distLeft).Select(x => "<").Aggregate((x, y) => x + y);
				}
			}

			var stateAtPlayersPosition = this.State[other._playerPosition];
			var desiredStateAtPlayersPosition = other.State[other._playerPosition];

			var distUp = this.GetRightDistance(stateAtPlayersPosition, desiredStateAtPlayersPosition, AlphabetHelper.AlphabetSize);
			var distDown = AlphabetHelper.AlphabetSize - distUp;

			if (distUp != 0 && distDown != 0)
			{
				if (distUp < distDown)
				{
					move += Enumerable.Range(0, distUp).Select(x => "+").Aggregate((x, y) => x + y);
				}
				else
				{
					move += Enumerable.Range(0, distDown).Select(x => "-").Aggregate((x, y) => x + y);
				}
			}

			move += ".";

			return move;
		}

		public int GetDistanceTo(BoardState other)
		{
			return this.State
				.Zip(other.State, (x, y) => Math.Min(
						   this.GetRightDistance(x, y, AlphabetHelper.AlphabetSize),
						   AlphabetHelper.AlphabetSize - this.GetRightDistance(x, y, AlphabetHelper.AlphabetSize)))
				.Sum()
				   //distance from player position to desired player position
				   + Math.Min(this.GetRightDistance(other._playerPosition, this._playerPosition, BoardSize), BoardSize - this.GetRightDistance(other._playerPosition, this._playerPosition, BoardSize));
		}

		private int GetRightDistance(int current, int desired, int length)
		{
			if (current < desired)
			{
				return desired - current;
			}

			var dist = current - desired;
			return length - dist;
		}
	}

	public class Sequence
	{
		private string _sequence;

		public Sequence()
		{
			_sequence = string.Empty;
		}

		public string GetSequence()
		{
			return _sequence;
		}

		public void MoveRight()
		{
			_sequence += ">";
		}

		public void MoveLeft()
		{
			_sequence += "<";
		}

		public void RollLetterUp()
		{
			_sequence += "+";
		}

		public void RollLetterDown()
		{
			_sequence += "-";
		}

		public void ActivateStone()
		{
			_sequence += ".";
		}
	}

	public static class AlphabetHelper
	{
		public const int AlphabetSize = 27;
		private const string Alphabet = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static int[] ToNumberArray(string phraze)
		{
			return phraze.Select(x => Alphabet.IndexOf(x)).ToArray();
		}

		public static int GetDistanceBetweenLetters(int x, int y)
		{
			if (x >= y)
			{
				return Math.Min(x - y, AlphabetSize - x + y);
			}

			return Math.Min(y - x, AlphabetSize + x - y);
		}

		public static string ToWord(int[] state)
		{
			return new string(state.Select(x => Alphabet[x]).ToArray());
		}

	}
}
