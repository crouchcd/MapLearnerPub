using System;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapLearner
{
	public sealed partial class AddHighScoreDialog : ContentDialog
	{
		private string highScoreName;

		public string HighScoreName
		{
			get
			{
				return highScoreName;
			}
		}

		public AddHighScoreDialog(TimeSpan timeSpan)
		{
			this.InitializeComponent();
			MessageTextBlock.Text = "Congratulations! You completed the map in: " + timeSpan + ". Enter your name to save your score:";
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			highScoreName = NameTextBox.Text;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}
