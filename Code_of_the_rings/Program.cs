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
		private static Sequence _sequence;

		static void Main(string[] args)
		{
			string magicPhrase = Console.ReadLine();
			// To debug: Console.Error.WriteLine("Debug messages...");

			_phraze = AlphabetConverter.ToNumberArray(magicPhrase);
			_zones = new int[30];
			_sequence = new Sequence();



			Console.WriteLine("+.>-.");
		}
	}

	public class Strategy
	{
		public string Hash { get; }

		private readonly BoardState _state;
		private readonly int _playerPosition;
		private int[] _remainingLetters;
		private int[] _parsedLetters;

		public Strategy(BoardState initialState, int[] remainingLetters, int[] parsedLetters, int playerPosition)
		{
			this._state = initialState;
			this._remainingLetters = remainingLetters;
			this._parsedLetters = parsedLetters;
			this._playerPosition = playerPosition;
			this.Hash = this.GetStrategyString();
		}

		//жадный алгоритм, на каждом шаге искать самое оптимальное решение
		public void MakeMove()
		{
			if (_remainingLetters.Length == 0)
			{
				throw new InvalidOperationException("Solution found, make something!!");
			}

			var allPosibleStates = new List<BoardState>();
			var nextLetter = _remainingLetters[0];

			allPosibleStates = _state.State.Select((x, i) => _state.Transform(i, x)).ToList();

			var bestState = allPosibleStates.GroupBy(x => x.GetDistanceTo(_state)).OrderBy(x => x.Key).First().First();

			var movement = _state.MoveTo(bestState);
			//need to change classes, Idea:
			//	- store movement, _remainingLetters letters and player position together,
			//	- then recuirsivelly make changes to the states, store AllowReversePInvokeCallsAttribute the descision tree somewhere,
			//		compare for dublicates and delete them.
			//	- if number of descisions > 1000 (for example)
			//		Select the best descision (compare by average number of moves for the letter (Moves.Length/numberOfPrintedLetters))
		}

		private string GetStrategyString()
		{
			return AlphabetConverter.ToWord(_state.State)
				   + _playerPosition
				   + AlphabetConverter.ToWord(_remainingLetters);
		}
	}

	public class BoardState
	{
		public const int BoardSize = 30;

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

			var distRight = Math.Abs(other.PlayerPosition - this.PlayerPosition);
			var distLeft = this.PlayerPosition + 1 + (BoardSize - other.PlayerPosition);

			if (distRight < distLeft)
			{
				move += Enumerable.Range(0, distRight).Select(x => ">").Aggregate((x, y) => x + y);
			}
			else
			{
				move += Enumerable.Range(0, distRight).Select(x => "<").Aggregate((x, y) => x + y);
			}

			var stateAtPlayersPosition = this.State[other.PlayerPosition];
			var desiredStateAtPlayersPosition = other.State[other.PlayerPosition];

			var distUp = Math.Abs(desiredStateAtPlayersPosition - stateAtPlayersPosition);
			var distDown = stateAtPlayersPosition + 1 + (BoardSize - desiredStateAtPlayersPosition);

			if (distUp < distDown)
			{
				move += Enumerable.Range(0, distUp).Select(x => "+").Aggregate((x, y) => x + y);
			}
			else
			{
				move += Enumerable.Range(0, distDown).Select(x => "-").Aggregate((x, y) => x + y);
			}

			move += ".";

			return move;
		}

		public int GetDistanceTo(BoardState other)
		{
			return this.State
				.Zip(other.State, (x, y) => Math.Min(Math.Abs(y - x), x + 1 + (BoardSize - y)))
				.Sum()
				   //distance from player position to desired player position
				   + Math.Min(Math.Abs(other.PlayerPosition - this.PlayerPosition), this.PlayerPosition + 1 + (BoardSize - other.PlayerPosition));
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
