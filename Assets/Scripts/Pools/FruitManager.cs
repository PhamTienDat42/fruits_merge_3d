using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Fruits;
using Game;
using ScriptableObjects;
using Services;
using TMPro;
using UnityEngine;

namespace Pools
{
	public class FruitManager : MonoBehaviour
	{
		[Header("ScriptableObject")]
		[SerializeField] private SOBallType ballTypes;
		[SerializeField] private SOBallType ballTypesNoPhysic;

		[SerializeField] private List<Fruit2D> fruitPrefabs;
		[SerializeField] private List<Fruit2D> combineFruitPrefabs;
		[SerializeField] private List<TMP_Text> bonusScoreTMPPrefabs;
		[SerializeField] private GameObject parentFruitPools;
		[SerializeField] private GameObject parentFruitCombinePools;
		[SerializeField] private GameObject parentBonusScorePools;

		[SerializeField] private GameController gameController;
		[SerializeField] private GameView gameView;
		[SerializeField] private GameModel gameModel;

		private Dictionary<int, ObjectPool<Fruit2D>> fruitPools;
		private Dictionary<int, ObjectPool<Fruit2D>> combinePools;
		private Dictionary<int, ObjectPool<TMP_Text>> bonusScorePools;

		private readonly int poolSizeForCombinePool = 10;
		private readonly int poolSizeForDrag = 1;

		private readonly float shakeForce = 5f;

		public event Action<Fruit2D> OnFruitCombinedFromPool;

		private int totalRandomScore = 300;
		private readonly List<int> randomStartScores = new();
		[SerializeField] private List<int> randomScores;
		[SerializeField] private List<int> bonusRandomScores;

		private ParamServices paramServices;

		private void Awake()
		{
			var index = PlayerPrefs.GetInt(Constants.Theme, 0);
			fruitPrefabs.Clear();
			combineFruitPrefabs.Clear();
			fruitPrefabs.AddRange(ballTypesNoPhysic.BallTypes[index].Fruit2D);
			combineFruitPrefabs.AddRange(ballTypes.BallTypes[index].Fruit2D);

			fruitPools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= Constants.FruitTypeCount; i++)
			{
				ObjectPool<Fruit2D> pool = new(() => Instantiate(fruitPrefabs[i - 1]), poolSizeForDrag, parentFruitPools);
				fruitPools.Add(i, pool);
			}

			combinePools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= Constants.FruitTypeCount; i++)
			{
				ObjectPool<Fruit2D> objectPool = new(() => DIFruit(i - 1), poolSizeForCombinePool, parentFruitCombinePools);
				combinePools.Add(i, objectPool);
			}
			randomStartScores.AddRange(randomScores);

			bonusScorePools = new Dictionary<int, ObjectPool<TMP_Text>>();
			for (int i = 1; i <= Constants.FruitTypeCount; i++)
			{
				ObjectPool<TMP_Text> objectPool = new(() => Instantiate(bonusScoreTMPPrefabs[i - 1]), poolSizeForCombinePool, parentBonusScorePools);
				bonusScorePools.Add(i, objectPool);
			}
		}

		private void Start()
		{
			randomStartScores.AddRange(randomScores);

			paramServices = gameController.GameServices.GetService<ParamServices>();
			if(paramServices.IsContinue == true)
			{
				gameModel.CurrentScore = PlayerPrefs.GetInt(Constants.OldScore, 0);
				LoadCombinePool();
			}
		}

		private Fruit2D DIFruit(int index)
		{
			Fruit2D fruit = Instantiate(combineFruitPrefabs[index]);
			fruit.InstantiateFruits(this, gameController, gameView, gameModel);
			return fruit;
		}

		private int RandomFruitToDrop()
		{
			var score = UnityEngine.Random.Range(0, totalRandomScore);
			var rateStep = 0;
			var fruitIndex = 0;

			for (var i = 0; i < randomScores.Count; ++i)
			{
				rateStep += randomScores[i];
				if (score < rateStep && fruitIndex == 0)
				{
					fruitIndex = i + 1;
					var temp = randomScores[i] - randomStartScores[i];
					randomScores[i] = randomStartScores[i];
					totalRandomScore -= temp;
					continue;
				}

				randomScores[i] += bonusRandomScores[i];
				totalRandomScore += bonusRandomScores[i];
			}
			return fruitIndex;
		}

		public Fruit2D GetNewFruitForShow(Vector3 pos)
		{
			int randomPoints = RandomFruitToDrop();
			Fruit2D newFruit = fruitPools[randomPoints].GetObject(pos);
			return newFruit;
		}

		public TMP_Text GetNewBonusScoreForShow(Vector3 pos)
		{
			int randomPoints = RandomFruitToDrop();
			TMP_Text bonusScore = bonusScorePools[randomPoints].GetObject(pos);
			return bonusScore;
		}

		private IEnumerator IShowBonusScore(int num, Vector3 pos)
		{
			var bonusScore = GetNewBonusScoreForShow(pos);
			bonusScore.text = $"+{num}";
			var bonusPos = new Vector3(pos.x, pos.y+1.0f, pos.z);
			bonusScore.transform.DOMove(bonusPos, 1.0f);
			yield return new WaitForSeconds(1.0f);
			bonusScore.gameObject.SetActive(false);
		}

		public void ShowBonusScore(int point, Vector3 pos)
		{
			StartCoroutine(IShowBonusScore(point, pos));
		}

		public Fruit2D GetFruitForDrop(int index, Vector3 pos)
		{
			ObjectPool<Fruit2D> combinePool = combinePools[index];
			int deactiveCount = 0;

			foreach (Fruit2D fruit in combinePool.GetObjectList())
			{
				if (!fruit.gameObject.activeSelf)
				{
					deactiveCount++;
				}
			}

			if (deactiveCount < 2)
			{
				Fruit2D newObj = Instantiate(combineFruitPrefabs[index - 1], parentFruitCombinePools.transform);
				newObj.InstantiateFruits(this, gameController, gameView, gameModel);
				newObj.transform.position = pos;
				combinePool.AddToPool(newObj);
			}

			Fruit2D combineFruit = combinePools[index].GetObject(pos);
			combineFruit.OnFruitCombined += OnFruitCombinedFromPool;
			Transform childTranform = combineFruit.GetComponent<Transform>().GetChild(0);
			StartCoroutine(RandomRotateFruits(childTranform, 3.0f));
			return combineFruit;
		}

		public Fruit2D GetFruitForContinue(int index)
		{
			ObjectPool<Fruit2D> combinePool = combinePools[index];
			int deactiveCount = 0;

			foreach (Fruit2D fruit in combinePool.GetObjectList())
			{
				if (!fruit.gameObject.activeSelf)
				{
					deactiveCount++;
				}
			}

			if (deactiveCount < 2)
			{
				Fruit2D newObj = Instantiate(combineFruitPrefabs[index - 1], parentFruitCombinePools.transform);
				newObj.InstantiateFruits(this, gameController, gameView, gameModel);
				combinePool.AddToPool(newObj);
			}

			Fruit2D combineFruit = combinePools[index].GetObject();
			return combineFruit;
		}

		private IEnumerator RandomRotateFruits(Transform fruitTransform, float duration)
		{
			float t = 0.0f;
			float randomX = UnityEngine.Random.Range(0.0f, 360.0f);
			float randomY = UnityEngine.Random.Range(0.0f, 360.0f);

			Quaternion startRot = fruitTransform.localRotation;
			Quaternion endRot = Quaternion.Euler(randomX, randomY, 0.0f);

			while (t < duration)
			{
				fruitTransform.localRotation = Quaternion.Lerp(startRot, endRot, t / duration);
				t += Time.deltaTime;
				yield return null;
			}
			fruitTransform.localRotation = endRot;
		}

		private void SaveFruitPoolJson(ObjectPool<Fruit2D> yourFruitPool)
		{
			List<Fruit2DData> fruitDataList = new();
			foreach (Fruit2D fruit in yourFruitPool.GetObjectList())
			{
				Fruit2DData fruitData = fruit.ToData();

				if(fruit.gameObject.activeSelf == true)
				{
					fruitDataList.Add(fruitData);
				}
			}
			SaveLoadManager.SaveFruitDataJson(fruitDataList);
		}

		private void LoadFruitPoolJson()
		{
			List<Fruit2DData> loadedFruitDataList = SaveLoadManager.LoadFruitDataFromJson();
			if (loadedFruitDataList != null)
			{
				for (int i = 0; i < loadedFruitDataList.Count; i++)
				{
					Fruit2DData fruitData = loadedFruitDataList[i];
					Fruit2D fruit = GetFruitForContinue(fruitData.FruitIndex);
					fruit.FromData(fruitData);
				}
			}
		}

		public void SaveCombinePool()
		{
			string filePath = Path.Combine(Application.persistentDataPath, Constants.FruitDataFileName);
			File.WriteAllText(filePath, string.Empty);
			foreach (var pool in combinePools)
			{
				SaveFruitPoolJson(pool.Value);
			}
		}

		public void LoadCombinePool()
		{
			LoadFruitPoolJson();
		}

		//Shake Phone
		public void ApplyShakeForce()
		{
			Vector2 shakeDirection = UnityEngine.Random.insideUnitCircle.normalized;
			Vector2 shakeForce = shakeDirection * this.shakeForce;
			var rigidList = new List<Rigidbody2D>();

			foreach(var pool in combinePools)
			{
				foreach (var fruit in pool.Value.GetObjectList())
				{
					if (fruit.gameObject.activeSelf)
					{
						rigidList.Add(fruit.Rb);
					}
				}

				foreach (var rb in rigidList)
				{
					rb.AddForce(shakeForce, ForceMode2D.Impulse);
				}
			}
		}

		public List<Fruit2D> FruitPrefabs => fruitPrefabs;
	}
}
