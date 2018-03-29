using System;

namespace MapLearner
{
	public static class MapLearnerRegionHelper
	{
		public static MapLearnerRegion convertStringToRegion(string region)
		{
			string str = region.Replace("  ", string.Empty).ToUpper();

			if (str == "UNITEDSTATES")
			{
				return MapLearnerRegion.UnitedStates;
			}
			else if (str == "NORTHAMERICA")
			{
				return MapLearnerRegion.NorthAmerica;
			}

			throw new ArgumentException("'" + region + "' is not a valid region.");
		}

		public static string convertRegionToString(MapLearnerRegion region, bool addSpaces = false)
		{
			switch (region)
			{
				case MapLearnerRegion.NorthAmerica:
					return addSpaces ? "North America" : "NorthAmerica";
				case MapLearnerRegion.UnitedStates:
					return addSpaces ? "United States" : "UnitedStates";
				default:
					throw new ArgumentException("Something has gone horribly wrong. This MapLearnerRegion could not be converted into string format.");
			}
		}

        public static MapLearnerRegion getRandomRegion()
		{
			Random random = new Random();
			MapLearnerRegion[] values = (MapLearnerRegion[])Enum.GetValues(typeof(MapLearnerRegion));
			return values[random.Next(0, values.Length)];
		}
	}
}
