using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;

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
		private readonly int[] _remainingLetters;

		public Strategy(BoardState initialState, int[] remainingLetters, int playerPosition)
		{

			this._state = initialState;
			this._remainingLetters = remainingLetters;
			this._playerPosition = playerPosition;
			this.Hash = this.GetHashCode();
		}

		public void MakeMove()
		{
			var moveFromCurrentPosition = _state.Transform(_playerPosition, _remainingLetters[0]);
			var moveFromLeftPosition = _state.Transform(_playerPosition - 1, _remainingLetters[0]);
			var moveFromRightPosition = _state.Transform(_playerPosition + 1, _remainingLetters[0]);

			need to change classes, Idea:
				- store movement, _remainingLetters letters and player position together,
				- then recuirsivelly make changes to the states, store AllowReversePInvokeCallsAttribute the descision tree somewhere,
					compare for dublicates and delete them.
				- if number of descisions > 1000 (for example)
					Select the best descision (compare by average number of moves for the letter (Moves.Length/numberOfPrintedLetters))
		}

		private string GetHashCode()
		{
			return AlphabetConverter.ToWord(_state.State)
				   + _playerPosition
				   + AlphabetConverter.ToWord(_remainingLetters);
		}
	}

	public class BoardState
	{
		public int[] State { get; }
		public string Move { get; private set; }

		public BoardState(int[] state)
		{
			State = state;
		}

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
			return new BoardState(stateCopy);
		}

		public int GetDistanceTo(BoardState other)
		{
			return this.State
				.Zip(other.State, (x, y) => Math.Min(y - x, x + 1 + (30 - y)))
				.Sum();
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
