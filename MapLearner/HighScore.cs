using System;

namespace MapLearner
{
	public enum HighScoreType
	{
		VoiceInput,
		TextInput
	}

	public class HighScore
	{
		public string Name { get; set; }
		public TimeSpan CompletedTime { get; set; }
		public HighScoreType Type { get; set; }
		public MapLearnerRegion Region { get; set; }
		public DateTime AchievedDateTime { get; set; }

		public void save()
		{
			XmlServiceClient client = XmlServiceClient.Instance;
			client.save(this);
		}

		public static bool isLegalName(string name)
		{
			return !string.IsNullOrWhiteSpace(name);
		}
	}
}