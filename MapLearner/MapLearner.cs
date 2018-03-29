using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.SpeechRecognition;

namespace MapLearner
{
	public class MapLearner
	{
		private int index;
		private XmlServiceClient xmlServiceClient;

		public List<State> States { get; }
		public MapLearnerRegion Region { get; }
		public State CurrentState
		{
			get { return States[index]; }
		}

		public MapLearner(MapLearnerRegion region, int numberOfStates = 10)
		{
			xmlServiceClient = XmlServiceClient.Instance;

			index = 0;
			Region = region;

			List<State> stateList = shuffleList(xmlServiceClient.getListOfStates(Region));
			States = randomlySelectItemsFromList(stateList, numberOfStates);
		}

		public MapLearner(SaveGame saveGame)
		{
			xmlServiceClient = XmlServiceClient.Instance;

			index = saveGame.getIndexOfCurrentState();
			Region = MapLearnerRegionHelper.convertStringToRegion(saveGame.Region);
			States = saveGame.States;
		}

		// Use this constructor to load a game
		public MapLearner(string saveName)
		{
			xmlServiceClient = XmlServiceClient.Instance;

			SaveGame saveGame = load(saveName);

			States = saveGame.States;
			Region = MapLearnerRegionHelper.convertStringToRegion(saveGame.Region);
			index = saveGame.getIndexOfCurrentState();
		}

		public bool guess(string state)
		{
			state = removeBlankSpaces(state);
			string currentStateName = removeBlankSpaces(CurrentState.Name);

			if (state.ToUpper() == currentStateName.ToUpper())
			{
				incrementIndex();
				return true;
			}

			return false;
		}

		public bool guess(SpeechRecognitionResult speechResult)
		{
			string currentStateName = removeBlankSpaces(CurrentState.Name);
            string speechResultName = removeBlankSpaces(speechResult.Text);

			if (speechResultName.ToUpper().Contains(currentStateName.ToUpper()))
			{
				incrementIndex();
				return true;
			}

			return false;
		}

		public bool contains(string state)
		{
			state = removeBlankSpaces(state);

			return States.Exists(x => x.Name.ToUpper() == state.ToUpper());
		}

		public bool find(string state)
		{
			state = removeBlankSpaces(state);

			if (this.contains(state))
			{
				State matchedState = States.Find(x => x.Name.ToUpper() == state.ToUpper());
				matchedState.IsFound = true;

				return true;
			}

			return false;
		}

		public bool isFound(string state)
		{
			state = removeBlankSpaces(state);

			if (this.contains(state))
			{
				State matchedState = States.Find(x => x.Name.ToUpper() == state.ToUpper());
				return matchedState.IsFound;
			}

			throw new ArgumentException("'" + state + "' does not exist in this region");
		}

		public bool allStatesAreFound()
		{
			return !States.Exists(x => x.IsFound == false);
		}

		public List<State> foundStates()
		{
			return States.Where(x => x.IsFound).ToList();
		}

		public List<State> unfoundStates()
		{
			return States.Where(x => !x.IsFound).ToList();
		}

		public static void save(MapLearner mapLearner, string saveName)
		{
			XmlServiceClient client = XmlServiceClient.Instance;
			client.save(getDataToSave(mapLearner, saveName));
		}

		public static void removeSaveGame(string saveName)
		{
			XmlServiceClient client = XmlServiceClient.Instance;
			client.remove(saveName);
		}

		public static MapLearner loadMostRecent()
		{
			return new MapLearner(XmlServiceClient.Instance.retrieveMostRecentSave());
		}

		private List<SaveGame> getSavedGameData()
		{
			return xmlServiceClient.SavedData.SaveGameData;
		}

		private SaveGame load(string saveName)
		{
			return xmlServiceClient.load(saveName);
		}

		private static SaveGame getDataToSave(MapLearner mapLearner, string saveName)
		{
			return new SaveGame
			{
				Name = saveName,
				CurrentState = mapLearner.CurrentState,
				States = mapLearner.States,
				Region = MapLearnerRegionHelper.convertRegionToString(mapLearner.Region)
			};
		}

		private void incrementIndex()
		{
			States[index].IsFound = true;

			if (index + 1 < States.Count)
			{
				index++;
			}
		}

		// Source : https://stackoverflow.com/questions/12180038/randomly-shuffle-a-list
		private List<T> shuffleList<T>(List<T> list)
		{
			Random random = new Random();

			return list.OrderBy(x => random.Next()).ToList();
		}

		private List<T> randomlySelectItemsFromList<T>(List<T> list, int numberOfItems)
		{
			if (numberOfItems > list.Count)
			{
				return list;
			}

			Random random = new Random();
			List<T> selectedItems = new List<T>();

			for (int i = 0; i < numberOfItems; i++)
			{
				int index = random.Next(list.Count);
				selectedItems.Add(list[index]);
				list.RemoveAt(index);
			}

			return selectedItems;
		}

		private string removeBlankSpaces(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				throw new ArgumentNullException();
			}

			return str.Replace(" ", string.Empty);
		}
	}
}