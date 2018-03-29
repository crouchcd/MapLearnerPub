using System.Collections.Generic;

namespace MapLearner
{
	public class SavedData
	{
		public List<SaveGame> SaveGameData { get; set; }
		public List<HighScore> HighScoresData { get; set; }

		public SavedData()
		{
			SaveGameData = new List<SaveGame>();
			HighScoresData = new List<HighScore>();
		}
	}
}