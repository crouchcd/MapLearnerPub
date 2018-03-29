using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MapLearner
{
	public sealed partial class MainPage : Page
    {
        public MainPage()
        {
			this.InitializeComponent();
			XmlServiceClient client = XmlServiceClient.Instance;
			//XmlServiceClient.clearSaveData();
		}

		private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
			this.Frame.Navigate(typeof(GamePage));
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

		private async void loadGameButton_Click(object sender, RoutedEventArgs e)
		{
			LoadDialog loadDialog = new LoadDialog();
			ContentDialogResult result = await loadDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				this.Frame.Navigate(typeof(GamePage), loadDialog.SaveGameName);
			}
		}

		private void continueButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(GamePage), "CONTINUE");
		}

		private async void highScoresButton_Click(object sender, RoutedEventArgs e)
		{
			HighScoresSelectionDialog selectionDialog = new HighScoresSelectionDialog();
			if (await selectionDialog.ShowAsync() == ContentDialogResult.Primary)
			{
				HighScoresDialog highScoresDialog = new HighScoresDialog(selectionDialog.Region);
				await highScoresDialog.ShowAsync();
			}
		}
	}
}