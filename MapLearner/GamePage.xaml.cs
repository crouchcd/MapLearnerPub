using System;
using System.Diagnostics;
using Windows.Media.SpeechRecognition;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapLearner
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePage : Page
    {
        private MapLearner mapLearner;
        private Stopwatch gameTimer = new Stopwatch();
        private bool TimedModeOn { get; set; }
        private bool SpeechModeOn { get; set; }

        public GamePage()
        {
            this.InitializeComponent();
            newGame(MapLearnerRegion.UnitedStates);
            SpeechModeOn = false;
            textInputModeButton.IsEnabled = false;
            changeInputModeView();
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			string parameter = e.Parameter as string;

			if (continueFromPrevious(parameter))
			{
				mapLearner = MapLearner.loadMostRecent();
			}
			else if (parameter == null)
			{
				
				mapLearner = new MapLearner(MapLearnerRegion.UnitedStates);
			}
			else
			{
				mapLearner = new MapLearner(parameter);
			}

			initializeBoard();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			// If this is internal navigation, we do not want to auto save
			if (e.SourcePageType != typeof(MainPage))
			{
				XmlServiceClient client = XmlServiceClient.Instance;
				MapLearner.save(mapLearner, client.AutoSaveName);
			}
		}

		private void newGame(MapLearnerRegion region)
        {
			mapLearner = new MapLearner(region);
			initializeBoard();
		}

		private void newGame()
        {
			mapLearner = new MapLearner(MapLearnerRegion.UnitedStates);
			initializeBoard();
		}

		private void initializeBoard()
		{
			TimedModeOn = false;
			setDefaultColors();
			textInputBox.Text = "";
			speechOutputBox.Text = "";
			mapImage.Source = mapLearner.CurrentState.getImage();
			regionLabel.Text = MapLearnerRegionHelper.convertRegionToString(mapLearner.Region, true);
			scoreOutputLabel.Text = mapLearner.foundStates().Count + " / " + mapLearner.States.Count;
		}

        #region Speech Functions //********************* SPEECH FUNCTIONS ****************************************************************

        // start a speech recognition session and capture the results
        private async void speechButton_Click(object sender, RoutedEventArgs e)
        {
            speechButton.Content = "Listening...";
            speechButton.IsEnabled = false;

            // Create an instance of SpeechRecognizer.
            using (var speechRecognizer = new SpeechRecognizer())
            {
                // Compile the dictation grammar by default.
                await speechRecognizer.CompileConstraintsAsync();

                // Start recognition.
                SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();

                // output result
                speechOutputBox.Text = speechRecognitionResult.Text;
                speechButton.Content = "Speak";
                speechButton.IsEnabled = true;
                //speechInputSubmitted(speechRecognitionResult);

                bool correctGuess = false;
                try
                {
                    correctGuess = mapLearner.guess(speechRecognitionResult);
                }
                catch (ArgumentNullException) { }
                finally
                {
                    AnswerSubmitted(correctGuess);
                }
            }
        }

        #endregion //********************* END SPEECH FUNCTIONS **********************************************************

        #region Text Input Functions //********************* TEXT INPUT FUNCTIONS ************************************************

        private void textInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            setDefaultColors();
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                bool correctGuess = false;
                try
                {
                    correctGuess = mapLearner.guess(textInputBox.Text);
                }
                catch (ArgumentNullException) { }
                finally
                {
                    AnswerSubmitted(correctGuess);
                }
                e.Handled = true;
            }
        }

        private void textInputEnterButton_Click(object sender, RoutedEventArgs e)
        {
            bool correctGuess = false;
            try
            {
                correctGuess = mapLearner.guess(textInputBox.Text);
            }
            catch (ArgumentNullException) { }
            finally
            {
                AnswerSubmitted(correctGuess);
            }
        }

        #endregion //********************* END TEXT INPUT FUNCTIONS **********************************************************

        #region SplitView Menu Handlers //********************* SPLIT VIEW MENU FUNCTIONS ************************************************

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            newGame(mapLearner.Region);
        }

        private void changeMapButton_Click(object sender, RoutedEventArgs e)
        {
            // right now this switches between U.S. and North American regions
            switch (mapLearner.Region)
            {
                case MapLearnerRegion.UnitedStates:
                    newGame(MapLearnerRegion.NorthAmerica);
                    break;
                case MapLearnerRegion.NorthAmerica:
                    newGame(MapLearnerRegion.UnitedStates);
                    break;
                default:
                    break;
            }
        }

        private void timedModeButton_Click(object sender, RoutedEventArgs e)
        {
            newGame(mapLearner.Region);
            showTimedModeOnMessageAsync();
        }

        private void speechInputModeButton_Click(object sender, RoutedEventArgs e)
        {
            speechInputModeButton.IsEnabled = false;
            textInputModeButton.IsEnabled = true;
            SpeechModeOn = true;
            changeInputModeView();
        }

        private void textInputModeButton_Click(object sender, RoutedEventArgs e)
        {
            speechInputModeButton.IsEnabled = true;
            textInputModeButton.IsEnabled = false;
            SpeechModeOn = false;
            changeInputModeView();
            textInputBox.Focus(FocusState.Programmatic);
        }

        private async void saveGameButton_Click(object sender, RoutedEventArgs e)
        {
            string text = string.Empty;
            SaveDialog saveDialog = new SaveDialog();
			ContentDialogResult result = await saveDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    MapLearner.save(mapLearner, saveDialog.Text);
                }
                catch (ArgumentException err)
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Saving Error:",
                        Content = err.Message,
                        PrimaryButtonText = "Ok"
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }

		private async void loadGameButton_Click(object sender, RoutedEventArgs e)
		{
			LoadDialog loadDialog = new LoadDialog();
			ContentDialogResult result = await loadDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				mapLearner = new MapLearner(loadDialog.SaveGameName);
				initializeBoard();
			}
		}

		private async void deleteSaveButton_Click(object sender, RoutedEventArgs e)
		{
			DeleteSaveDialog deleteSaveDialog = new DeleteSaveDialog();
			ContentDialogResult result = await deleteSaveDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				ContentDialog locationPromptDialog = new ContentDialog
				{
					Title = "Are you sure you want to delete save '" + deleteSaveDialog.DeleteSaveName + "'?",
					Content = "Once deleted, your save will be lost forever!",
					SecondaryButtonText = "Cancel",
					PrimaryButtonText = "Yes!"
				};

				if (await locationPromptDialog.ShowAsync() == ContentDialogResult.Primary)
				{
					MapLearner.removeSaveGame(deleteSaveDialog.DeleteSaveName);

					var messageDialog = new Windows.UI.Popups.MessageDialog(deleteSaveDialog.DeleteSaveName + " has been deleted.", "Success!");
					await messageDialog.ShowAsync();
				}
			}
		}

		#endregion //********************* END SPLIT VIEW MENU FUNCTIONS ************************************************

		#region Helper Functions //********************* HELPER FUNCTIONS ************************************************

		// processes the GUI actions after an answer is submitted via text or voice
		private void AnswerSubmitted(bool correctGuess)
		{
            if (SpeechModeOn)
            {
                if (correctGuess)
                {
                    scoreOutputLabel.Foreground = new SolidColorBrush(Colors.Green);
                    speechOutputBox.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    speechOutputBox.Foreground = new SolidColorBrush(Colors.DarkRed);
                }
            }
            else
            {
                if (correctGuess)
                {
                    scoreOutputLabel.Foreground = new SolidColorBrush(Colors.Green);
                    textInputBox.Text = "";
                }
                else
                {
                    textInputBox.Background = new SolidColorBrush(Colors.DarkRed);
                }
            }

            if (correctGuess)
            {
                mapImage.Source = mapLearner.CurrentState.getImage();
                updateScore();

                if (mapLearner.allStatesAreFound() && !TimedModeOn)
                {
                    showMapCompleteMessageAsync();
                }
                else if (mapLearner.allStatesAreFound() && TimedModeOn)
                {
                    gameTimer.Stop();
                    showTimedMapCompleteMessageAsync();
                }
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void updateScore()
        {
            scoreOutputLabel.Text = mapLearner.foundStates().Count + " / " + mapLearner.States.Count;
        }

        // show a dialog indicating that the user completed the map w/out timed mode
        private async void showMapCompleteMessageAsync()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Map Complete",
                Content = "Congratulations! You completed the map.",
                PrimaryButtonText = "Ok"
            };

            ContentDialogResult result = await dialog.ShowAsync();
        }

        // show a dialog indicating that the user completed the map w/ timed mode
        private async void showTimedMapCompleteMessageAsync()
        {
			DateTime completedDateTime = DateTime.Now;
			AddHighScoreDialog addHighScoreDialog = new AddHighScoreDialog(gameTimer.Elapsed);

			if (await addHighScoreDialog.ShowAsync() == ContentDialogResult.Primary)
			{
				HighScore highScore = new HighScore
				{
					Name = addHighScoreDialog.HighScoreName,
					CompletedTime = gameTimer.Elapsed,
					AchievedDateTime = completedDateTime,
					Region = mapLearner.Region
				};

				highScore.save();
			}
		}

        // show a dialog before turning on timed game mode
        private async void showTimedModeOnMessageAsync()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Timed Mode",
                Content = "Are you ready to start the game?",
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                MySplitView.IsPaneOpen = false;
                textInputBox.Focus(FocusState.Programmatic);
                TimedModeOn = true;
                gameTimer.Reset();
                gameTimer.Start();
            }
            else
            {
                TimedModeOn = false;
            }
        }

        // Resets default colors of textInputBox and ouputScoreText
        private void setDefaultColors()
        {
            textInputBox.Background = new SolidColorBrush(Colors.White);
            scoreOutputLabel.Foreground = new SolidColorBrush(Colors.Black);
        }

        // Changes GUI to reflect the current input mode
        private void changeInputModeView()
        {
            if (SpeechModeOn)
            {
                textBorder.Background = new SolidColorBrush(Colors.LightGray);
                speechBorder.Background = null;
            }
            else
            {
                speechBorder.Background = new SolidColorBrush(Colors.LightGray);
                textBorder.Background = null;
            }
        }

		private bool continueFromPrevious(string parameter)
		{
			if (App.ReturnFromSavingState)
			{
				return true;
			}
			else if (string.IsNullOrWhiteSpace(parameter))
			{
				return false;
			}

			return parameter == "CONTINUE";
		}
		#endregion //********************* END HELPER FUNCTIONS 
	}
}