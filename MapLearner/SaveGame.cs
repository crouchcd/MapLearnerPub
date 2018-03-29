using System;
using System.Collections.Generic;

namespace MapLearner
{
	public class SaveGame
	{
		public string Name { get; set; }
		public string Region { get; set; }
		public State CurrentState { get; set; }
		public List<State> States { get; set; }
		public DateTime CreatedDateTime { get; set; }

		// Default constructor
		public SaveGame()
		{
			CreatedDateTime = DateTime.Now;
		}

		// Copy constructor (performs a shallow copy)
		public SaveGame(SaveGame other)
		{
			CreatedDateTime = other.CreatedDateTime;
			Name = other.Name;
			Region = other.Region;
			CurrentState = new State(other.CurrentState);

			States = new List<State>();
			foreach (State state in other.States)
			{
				States.Add(new State(state));
			}
		}

		public int getIndexOfCurrentState()
		{
			for (int i = 0; i < States.Count; i++)
			{
				if (States[i].Equals(CurrentState))
				{
					return i;
				}
			}

			return -1;
		}
	}
}