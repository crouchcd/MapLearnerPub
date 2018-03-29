using System;
using Windows.UI.Xaml.Media.Imaging;

namespace MapLearner
{
	// State Images Source: https://www.50states.com/
	public class State
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Capital { get; set; }
		public bool IsFound { get; set; }
		public string ImageLocation { get; set; }

		// Default Constructor
		public State() { }

		// Copy Constructor
		public State(State other)
		{
			Id = other.Id;
			Name = other.Name;
			Capital = other.Capital;
			IsFound = other.IsFound;
			ImageLocation = other.ImageLocation;
		}

		public BitmapImage getImage()
		{
			return new BitmapImage(new Uri(@"ms-appx://" + ImageLocation, UriKind.Absolute));
		}

		public bool Equals(State other)
		{
			return
				Id == other.Id &&
				Name == other.Name &&
				Capital == other.Capital &&
				IsFound == other.IsFound &&
				ImageLocation == other.ImageLocation;
		}
	}
}