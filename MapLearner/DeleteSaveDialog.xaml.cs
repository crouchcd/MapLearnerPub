using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapLearner
{
	public sealed partial class DeleteSaveDialog : ContentDialog
	{
		private string deleteSaveName;
		private List<SaveGame> saveGames;

		public string DeleteSaveName
		{
			get { return deleteSaveName; }
		}

		public DeleteSaveDialog()
		{
			this.InitializeComponent();

			deleteSaveName = string.Empty;
			XmlServiceClient xmlServiceClient = XmlServiceClient.Instance;
			saveGames = xmlServiceClient.SavedData.SaveGameData;

			SaveGameList.ItemsSource = saveGames;
            if (saveGames.Count == 0)
                this.IsPrimaryButtonEnabled = false;
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			deleteSaveName = ((SaveGame)SaveGameList.SelectedItem).Name;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}
