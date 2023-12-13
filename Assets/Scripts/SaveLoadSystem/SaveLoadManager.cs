using System.Collections.Generic;
using System.IO;
using Fruits;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoadSystem
{
	public class SaveLoadManager : MonoBehaviour
	{
		public static void SaveFruitDataJson(List<Fruit2DData> fruitDataList)
		{
			string filePath = Path.Combine(Application.persistentDataPath, Constants.FruitDataFileName);

			List<Fruit2DData> existingData = new();
			if (File.Exists(filePath))
			{
				string jsonData = File.ReadAllText(filePath);
				if (!string.IsNullOrEmpty(jsonData))
				{
					existingData = JsonConvert.DeserializeObject<List<Fruit2DData>>(jsonData);
				}
			}
			existingData.AddRange(fruitDataList);

			string updatedJsonData = JsonConvert.SerializeObject(existingData, Formatting.Indented);
			File.WriteAllText(filePath, updatedJsonData);
		}

		public static List<Fruit2DData> LoadFruitDataFromJson()
		{
			string filePath = Path.Combine(Application.persistentDataPath, Constants.FruitDataFileName);
			List<Fruit2DData> loadedData = new();

			if (File.Exists(filePath))
			{
				string jsonData = File.ReadAllText(filePath);

				if (!string.IsNullOrEmpty(jsonData))
				{
					loadedData = JsonConvert.DeserializeObject<List<Fruit2DData>>(jsonData);
				}
			}
			return loadedData;
		}

		public static void SaveSingleFruitDataJson(Fruit2DData fruitData)
		{
			string jsonData = JsonUtility.ToJson(fruitData);
			string filePath = Path.Combine(Application.persistentDataPath, Constants.FruitDataFileName);
			File.WriteAllText(filePath, jsonData);
		}
	}
}
