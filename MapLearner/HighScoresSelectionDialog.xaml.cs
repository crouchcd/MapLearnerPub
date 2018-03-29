using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapLearner
{
	public sealed partial class HighScoresSelectionDialog : ContentDialog
	{
		public MapLearnerRegion Region
		{
			get
			{
				if (unitedStatesButton.IsChecked.GetValueOrDefault())
				{
					return MapLearnerRegion.UnitedStates;
				}
				else
				{
					return MapLearnerRegion.NorthAmerica;
				}
			}
		}

		public HighScoresSelectionDialog()
		{
			this.InitializeComponent();
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}
