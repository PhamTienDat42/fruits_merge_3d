using Fruits;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
	private const string SaveFileName = "FruitData.dat";

	public static void SaveFruitData(Fruit2D fruit)
	{
		Fruit2DData fruitData = Fruit2DData.FromFruit(fruit);
		BinaryFormatter formatter = new();
		string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

		using (FileStream stream = new(filePath, FileMode.Create))
		{
			formatter.Serialize(stream, fruitData);
		}
	}

	public static void SaveFruitData(List<Fruit2DData> fruitDataList)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

		using (FileStream stream = new FileStream(filePath, FileMode.Create))
		{
			formatter.Serialize(stream, fruitDataList);
		}
	}

	public static Fruit2DData LoadFruitData()
	{
		string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

		if (File.Exists(filePath))
		{
			BinaryFormatter formatter = new();
			using (FileStream stream = new(filePath, FileMode.Open))
			{
				Fruit2DData fruitData = (Fruit2DData)formatter.Deserialize(stream);
				return fruitData;
			}
		}
		else
		{
			Logger.Debug($"File doesn't exist in {filePath}");
		}

		return null;
	}

	public static List<Fruit2DData> LoadFruitDataPool()
	{
		string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

		if (File.Exists(filePath))
		{
			BinaryFormatter formatter = new();
			using (FileStream stream = new FileStream(filePath, FileMode.Open))
			{
				List<Fruit2DData> fruitDataList = (List<Fruit2DData>)formatter.Deserialize(stream);
				return fruitDataList;
			}
		}
		else
		{
			Debug.LogWarning($"File doesn't exist in {filePath}");
		}

		return null;
	}
}
