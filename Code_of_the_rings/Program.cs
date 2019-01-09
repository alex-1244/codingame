using System;
using System.Linq;

namespace Code_of_the_rings
{
	//https://www.codingame.com/ide/puzzle/code-of-the-rings
	class Program
	{
		private static int[] _phraze;
		private static int[] _zones;
		private static Sequence _sequence;

		static void Main(string[] args)
		{
			string magicPhrase = Console.ReadLine();
			// To debug: Console.Error.WriteLine("Debug messages...");

			_phraze = AlphabetConverter.ToNumberArray(magicPhrase);
			_zones = new int[30];
			_sequence = new Sequence();

			var state = new BoardState(_zones, 0);
			var solution = string.Empty;

			foreach (var letter in _phraze)
			{
				Strategy s = new Strategy(state);
				var nextMove = s.Solve(state, letter);
				solution += nextMove.path;
				state = nextMove.state;
			}

			Console.WriteLine(solution);
		}
	}

	public class Strategy
	{
		public string Hash { get; }

		private readonly BoardState _state;

		public Strategy(BoardState initialState)
		{
			this._state = initialState;
			this.Hash = this.GetStrategyString();
		}

		public (BoardState state, string path) Solve(BoardState state, int nextLetter)
		{
			var allPosibleStates = _state.State.Select((x, i) => _state.Transform(i, nextLetter)).ToList();

			var bestState = allPosibleStates.GroupBy(x => x.GetDistanceTo(_state)).OrderBy(x => x.Key).First().First();

			var movement = _state.MoveTo(bestState);

			return (bestState, movement);
		}

		private string GetStrategyString()
		{
			return AlphabetConverter.ToWord(_state.State)
				   + _state.PlayerPosition;
			//+ AlphabetConverter.ToWord(_remainingLetters);
		}
	}

	public class BoardState
	{
		public const int BoardSize = 30;
		public const int AlphabetSize = 27;

		public BoardState(int[] state, int playerPosition)
		{
			State = state;
			PlayerPosition = playerPosition;
		}

		public int[] State { get; }
		public int PlayerPosition { get; private set; }

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

			var distRight = this.GetRightDistance(this.PlayerPosition, other.PlayerPosition, BoardSize);
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

			var stateAtPlayersPosition = this.State[other.PlayerPosition];
			var desiredStateAtPlayersPosition = other.State[other.PlayerPosition];

			var distUp = this.GetRightDistance(stateAtPlayersPosition, desiredStateAtPlayersPosition, AlphabetSize);
			var distDown = AlphabetSize - distUp;

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
				.Zip(other.State, (x, y) => Math.Min(this.GetRightDistance(x, y, AlphabetSize), AlphabetSize - this.GetRightDistance(x, y, AlphabetSize)))
				.Sum()
				   //distance from player position to desired player position
				   + Math.Min(this.GetRightDistance(other.PlayerPosition, this.PlayerPosition, BoardSize), BoardSize - this.GetRightDistance(other.PlayerPosition, this.PlayerPosition, BoardSize));
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

		//private int GetLeftDistance(int current, int desired, int length)
		//{
		//	if (desired < current)
		//	{
		//		return current - desired;
		//	}

		//	var dist = current - desired;
		//	return length - dist;
		//}
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

	public static class AlphabetConverter
	{
		private const string Alphabet = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static int[] ToNumberArray(string phraze)
		{
			return phraze.Select(x => Alphabet.IndexOf(x)).ToArray();
		}

		public static string ToWord(int[] state)
		{
			return new string(state.Select(x => Alphabet[x]).ToArray());
		}

	}
}
