using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLearner
{
	// NOTE: This class is to be removed from the program as soon as possible
	public class MapLearnerStubBuilder
	{
		public static List<State> getTestData(MapLearnerRegion region)
		{
			switch (region)
			{
				case MapLearnerRegion.UnitedStates:
					return getUnitedStatesTestData();
				case MapLearnerRegion.NorthAmerica:
					return getNorthAmericaTestData();
				default:
					throw new ArgumentException("Something has gone horribly wrong. A MapLearner for this region could not be created");
			}
		}

		private static List<State> getUnitedStatesTestData()
		{
			State Texas = new State()
			{
				Id = 1,
				Name = "Texas",
				Capital = "Austin",
				IsFound = false
			};

			State NewYork = new State()
			{
				Id = 2,
				Name = "New York",
				Capital = "Albany",
				IsFound = false
			};

			State Arkansas = new State()
			{
				Id = 1,
				Name = "Arkansas",
				Capital = "Little Rock",
				IsFound = false
			};

			State Oklahoma = new State()
			{
				Id = 1,
				Name = "Oklahoma",
				Capital = "Oklahoma City",
				IsFound = false
			};

			State California = new State()
			{
				Id = 1,
				Name = "California",
				Capital = "Sacramento",
				IsFound = false
			};

			return new List<State>() { Texas, NewYork, Arkansas, Oklahoma, California };
		}

		private static List<State> getNorthAmericaTestData()
		{
			throw new NotImplementedException("This functionality has not been implemented yet! I should probably fix that...");
		}
	}
}