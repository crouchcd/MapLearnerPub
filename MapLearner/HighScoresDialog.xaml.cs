using System.Linq;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapLearner
{
	public sealed partial class HighScoresDialog : ContentDialog
	{
		public HighScoresDialog(MapLearnerRegion region)
		{
			this.InitializeComponent();
			XmlServiceClient client = XmlServiceClient.Instance;

			HighScoresList.ItemsSource = client.SavedData.HighScoresData.Where(x => x.Region == region).OrderBy(x => x.CompletedTime);
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }
	}
}