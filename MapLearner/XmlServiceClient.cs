using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MapLearner
{
	class XmlServiceClient
	{
		private static XmlServiceClient instance;

		private const string autoSaveName = "AutoSave";
		private const string fileLocationsPath = @"Assets/Configurations/FileLocations.xml";
		private const string savedDataFileName = @"SavedData.xml";
		private readonly Dictionary<MapLearnerRegion, string> flieLocationsDictionary;

		private XmlServiceClient()
		{
			flieLocationsDictionary = getFileLocationsDictionary();

			SavedData = retrieveSavedData().Result;
		}

		public SavedData SavedData { get; }
		public string AutoSaveName
		{
			get
			{
				return autoSaveName;
			}
		}
		public static XmlServiceClient Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new XmlServiceClient();
				}

				return instance;
			}
		}

		public void save(SaveGame save)
		{
			SaveGame copy = save;

			if (save.Name == AutoSaveName)
			{
				remove(AutoSaveName);
			}
			else if (saveNameIsIllegal(save.Name))
			{
				throw new ArgumentException("A save with the name '" + save.Name + "' is not allowed");
			}
			else if (SavedData.SaveGameData.Any(x => x.Name == save.Name))
			{
				throw new ArgumentException("A save with the name '" + save.Name + "' already exists");
			}

			SavedData.SaveGameData.Add(new SaveGame(save));
			writeToXml(SavedData, savedDataFileName);
		}

		public static async void clearSaveData()
		{
			await ApplicationData.Current.LocalFolder.DeleteAsync(StorageDeleteOption.Default);
		}

		public void save(HighScore highScore)
		{
			SavedData.HighScoresData.Add(highScore);
			writeToXml(SavedData, savedDataFileName);
		}

		public void remove(string saveName)
		{
			if (saveExists(saveName))
			{
				SaveGame saveGameToRemove = SavedData.SaveGameData.First(x => x.Name == saveName);
				SavedData.SaveGameData.Remove(saveGameToRemove);

				writeToXml(SavedData, savedDataFileName);
			}
		}

		public SaveGame load(string name)
		{
			return SavedData.SaveGameData.First(x => x.Name == name);
		}

		public SaveGame retrieveMostRecentSave()
		{
			return SavedData.SaveGameData.OrderBy(x => x.CreatedDateTime).Last();
		}

		public List<State> getListOfStates(MapLearnerRegion type)
		{
			string fileLocation = getFileLocation(type);

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<State>));
			using (XmlReader reader = getXDocument(fileLocation).Root.CreateReader())
			{
				return (List<State>)xmlSerializer.Deserialize(reader);
			}
		}

		private async Task<SavedData> retrieveSavedData()
		{
			SavedData savedData = new SavedData();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedData));

			try
			{
				StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(savedDataFileName);

				using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
				{
					if (writeStream.Size != 0)
					{
						using (XmlReader xmlReader = XmlReader.Create(writeStream.AsStreamForRead()))
						{
							savedData = xmlSerializer.Deserialize(xmlReader) as SavedData;

							if (savedData == null)
							{
								savedData = new SavedData();
							}
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
				// EAT: The save game file doesn't exist, so a new one will be made later
			}

			return savedData;
		}

		private bool saveExists(string saveName)
		{
			return SavedData.SaveGameData.Any(x => x.Name == saveName);
		}

		private async void writeToXml(SavedData savedData, string fileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(savedData.GetType());
			StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

			using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(writeStream.AsStreamForWrite()))
				{
					xmlSerializer.Serialize(xmlWriter, savedData);
				}
			}
		}

		private string getFileLocation(MapLearnerRegion type)
		{
			if (flieLocationsDictionary == null)
			{
				throw new ArgumentNullException("The flieLocationsDictionary is currently null");
			}

			return flieLocationsDictionary[type];
		}

		private Dictionary<MapLearnerRegion, string> getFileLocationsDictionary()
		{
			Dictionary<MapLearnerRegion, string> flieLocationsDictionary = new Dictionary<MapLearnerRegion, string>();

			XPathDocument fileLocaitonsDocument = getXPathDocument(fileLocationsPath);
			XPathNavigator navigator = fileLocaitonsDocument.CreateNavigator();
			XPathNodeIterator iterator = navigator.Select("/fileLocations/fileLocation");

			foreach (XPathNavigator value in iterator)
			{
				string strCurrentType = value.SelectSingleNode(navigator.Compile("type")).Value;
				MapLearnerRegion type = MapLearnerRegionHelper.convertStringToRegion(strCurrentType);

				string fileLocation = value.SelectSingleNode(navigator.Compile("filePath")).Value;

				flieLocationsDictionary.Add(type, fileLocation);
			}

			return flieLocationsDictionary;
		}

		private XPathDocument getXPathDocument(string filePath)
		{
			XPathDocument document;

			try
			{
				document = new XPathDocument(filePath);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured while loading the Xml Document from: '" + filePath + "'. Details: " + ex.Message);
			}

			return document;
		}

		private XDocument getXDocument(string filePath)
		{
			XDocument document;

			try
			{
				document = XDocument.Load(filePath);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured while loading the Xml Document from: '" + filePath + "'. Details: " + ex.Message);
			}

			return document;
		}

		private bool saveNameIsIllegal(string str)
		{
			str = str.Trim();

			return 
				str == "CONTINUE" || 
				str == "NEW_GAME" || 
				string.IsNullOrWhiteSpace(str);
		}
	}
}