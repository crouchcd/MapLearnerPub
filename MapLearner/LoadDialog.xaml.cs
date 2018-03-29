using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapLearner
{
	public sealed partial class LoadDialog : ContentDialog
	{
		private string saveGameName;
		private List<SaveGame> saveGames;

		public string SaveGameName
		{
			get { return saveGameName;  }
		}

		public LoadDialog()
		{
			this.InitializeComponent();

			saveGameName = string.Empty;
			XmlServiceClient xmlServiceClient = XmlServiceClient.Instance;
			saveGames = xmlServiceClient.SavedData.SaveGameData;

			SaveGameList.ItemsSource = saveGames;
            if (saveGames.Count == 0)
			{
				IsPrimaryButtonEnabled = false;
			}
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			saveGameName = ((SaveGame)SaveGameList.SelectedItem).Name;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}