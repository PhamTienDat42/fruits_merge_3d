using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Fruits;
using Game;
using SaveLoadSystem;
using ScriptableObjects;
using Services;
using TMPro;
using UnityEngine;

namespace Pools
{
	public class FruitPoolManager : MonoBehaviour
	{

		[Header("Systems")]
		[SerializeField] private GameController gameController;
		[SerializeField] private GameView gameView;
		[SerializeField] private GameModel gameModel;

		[Space(8.0f)]
		[Header("ScriptableObject")]
		[SerializeField] private SOBallType ballTypes;
		[SerializeField] private SOBallType ballTypesNoPhysic;

		[Space(8.0f)]
		[Header("Lists")]
		[SerializeField] private List<Fruit2D> ballNoPhysicPrefabs;
		[SerializeField] private List<Fruit2D> ballPhysicPrefabs;
		[SerializeField] private List<TMP_Text> bonusScoreTMPPrefabs;
		[SerializeField] private List<int> randomScores;
		[SerializeField] private List<int> bonusRandomScores;

		[Space(8.0f)]
		[Header("GameObeject parents")]
		[SerializeField] private GameObject parentBallNoPhysic;
		[SerializeField] private GameObject parentBallPhysics;
		[SerializeField] private GameObject parentBonusScorePools;

		private Dictionary<int, ObjectPool<Fruit2D>> ballNoPhysicBools;
		private Dictionary<int, ObjectPool<Fruit2D>> ballPhysicBools;
		private Dictionary<int, ObjectPool<TMP_Text>> bonusScorePools;

		private readonly int poolSizePhysic = 10;
		private readonly int poolSizeNoPhysic = 1;

		private readonly float shakeForce = 5f;

		private int totalRandomScore = 300;
		private readonly List<int> randomStartScores = new();

		public event Action<Fruit2D> OnFruitCombinedFromPool;

		private ParamServices paramServices;

		private void Awake()
		{
			var themeIndex = PlayerPrefs.GetInt(Constants.Theme, 0);
			BilliardThemeIndex = themeIndex == 0;
			ballNoPhysicPrefabs.Clear();
			ballPhysicPrefabs.Clear();
			ballNoPhysicPrefabs.AddRange(ballTypesNoPhysic.BallTypes[themeIndex].Fruit2D);
			ballPhysicPrefabs.AddRange(ballTypes.BallTypes[themeIndex].Fruit2D);

			CreateNewBallNoPhysicPoolDictionary();
			CreateNewPhysicBallPoolDictionary();
			CreateNewBonusScorePoolDictinary();
			randomStartScores.AddRange(randomScores);
		}

		private void Start()
		{
			randomStartScores.AddRange(randomScores);

			paramServices = gameController.GameServices.GetService<ParamServices>();
			if (paramServices.IsContinue == true)
			{
				gameModel.CurrentScore = PlayerPrefs.GetInt(Constants.OldScore, 0);
				LoadCombinePool();
			}
		}

		private void CreateNewPhysicBallPoolDictionary()
		{
			ballPhysicBools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= Constants.MaxFruitType; i++)
			{
				ObjectPool<Fruit2D> objectPool = new(() => InstantiateBallHas(i - 1), poolSizePhysic, parentBallPhysics);
				ballPhysicBools.Add(i, objectPool);
			}
		}

		private Fruit2D InstantiateBallHas(int index)
		{
			Fruit2D fruit = Instantiate(ballPhysicPrefabs[index]);
			fruit.InstantiateFruits(this, gameController, gameView, gameModel);
			return fruit;
		}

		private void CreateNewBallNoPhysicPoolDictionary()
		{
			ballNoPhysicBools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= Constants.MaxFruitType; i++)
			{
				ObjectPool<Fruit2D> pool = new(() => Instantiate(ballNoPhysicPrefabs[i - 1]), poolSizeNoPhysic, parentBallNoPhysic);
				ballNoPhysicBools.Add(i, pool);
			}
		}

		private void CreateNewBonusScorePoolDictinary()
		{
			bonusScorePools = new Dictionary<int, ObjectPool<TMP_Text>>();
			for (int i = 1; i <= Constants.MaxFruitType; i++)
			{
				ObjectPool<TMP_Text> objectPool = new(() => Instantiate(bonusScoreTMPPrefabs[i - 1]), poolSizePhysic, parentBonusScorePools);
				bonusScorePools.Add(i, objectPool);
			}
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

		public Fruit2D GetNewBallNoPhysicOn(Vector3 pos)
		{
			int randomPoints = RandomFruitToDrop();
			Fruit2D newFruit = ballNoPhysicBools[randomPoints].GetObject(pos);
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
			var bonusPos = new Vector3(pos.x, pos.y + 1.0f, pos.z);
			bonusScore.transform.DOMove(bonusPos, 1.0f);
			yield return new WaitForSeconds(1.0f);
			bonusScore.gameObject.SetActive(false);
		}

		public void ShowBonusScore(int point, Vector3 pos)
		{
			StartCoroutine(IShowBonusScore(point, pos));
		}

		public void GetNewCombineBall(int index, Vector3 pos)
		{
			StartCoroutine(IGetNewPhysicBall(index, pos));
		}

		private IEnumerator IGetNewPhysicBall(int index, Vector3 pos)
		{
			yield return null;
			GetNewPhysicBallHas(index, pos);
		}

		public Fruit2D GetNewPhysicBallHas(int index, Vector3 pos)
		{
			ObjectPool<Fruit2D> combinePool = ballPhysicBools[index];

			if (CountBallsDeactiveIn(combinePool) < 2)
			{
				var newPhysicBall = InstantiateNewPhysicBallHas(index, pos);
				combinePool.AddToPool(newPhysicBall);
			}

			Fruit2D combineFruit = ballPhysicBools[index].GetObject(pos);
			combineFruit.OnFruitCombined += OnFruitCombinedFromPool;
			Transform childTranform = combineFruit.GetComponent<Transform>().GetChild(0);
			StartCoroutine(RandomRotateFruits(childTranform, 3.0f));
			return combineFruit;
		}

		private int CountBallsDeactiveIn(ObjectPool<Fruit2D> poolPhysicBalls)
		{
			int deactiveCount = 0;
			foreach (Fruit2D fruit in poolPhysicBalls.GetObjectList())
			{
				if (!fruit.gameObject.activeSelf)
				{
					deactiveCount++;
				}
			}
			return deactiveCount;
		}

		private Fruit2D InstantiateNewPhysicBallHas(int index, Vector3 pos)
		{
			Fruit2D newPhysicBall = Instantiate(ballPhysicPrefabs[index - 1], parentBallPhysics.transform);
			newPhysicBall.InstantiateFruits(this, gameController, gameView, gameModel);
			newPhysicBall.transform.position = pos;
			return newPhysicBall;
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

		public void SaveCombinePool()
		{
			string filePath = Path.Combine(Application.persistentDataPath, Constants.FruitDataFileName);
			File.WriteAllText(filePath, string.Empty);
			foreach (var pool in ballPhysicBools)
			{
				SaveFruitPoolJson(pool.Value);
			}
		}

		private void SaveFruitPoolJson(ObjectPool<Fruit2D> yourFruitPool)
		{
			List<Fruit2DData> fruitDataList = new();
			foreach (Fruit2D fruit in yourFruitPool.GetObjectList())
			{
				Fruit2DData fruitData = fruit.ToData();

				if (fruit.gameObject.activeSelf == true)
				{
					fruitDataList.Add(fruitData);
				}
			}
			SaveLoadManager.SaveFruitDataJson(fruitDataList);
		}

		public void LoadCombinePool()
		{
			LoadFruitPoolJson();
		}

		private void LoadFruitPoolJson()
		{
			List<Fruit2DData> loadedFruitDataList = SaveLoadManager.LoadFruitDataFromJson();
			if (loadedFruitDataList != null)
			{
				for (int i = 0; i < loadedFruitDataList.Count; i++)
				{
					Fruit2DData fruitData = loadedFruitDataList[i];
					Fruit2D fruit = GetBallWhenContinue(fruitData.FruitIndex);
					fruit.FromData(fruitData);
				}
			}
		}

		public Fruit2D GetBallWhenContinue(int index)
		{
			ObjectPool<Fruit2D> combinePool = ballPhysicBools[index];

			if (CountBallsDeactiveIn(combinePool) < 2)
			{
				Fruit2D newObj = Instantiate(ballPhysicPrefabs[index - 1], parentBallPhysics.transform);
				newObj.InstantiateFruits(this, gameController, gameView, gameModel);
				combinePool.AddToPool(newObj);
			}

			Fruit2D combineFruit = ballPhysicBools[index].GetObject();
			return combineFruit;
		}

		public void ReturnToPool()
		{
			foreach (var pool in ballPhysicBools)
			{
				foreach (Fruit2D fruit in pool.Value.GetObjectList())
				{
					fruit.gameObject.SetActive(false);
				}
			}
		}

		//Shake Phone
		public void ApplyShakeForceToPoolBall()
		{
			var shakeDirection = UnityEngine.Random.insideUnitCircle.normalized;
			var newShakeForce = shakeDirection * shakeForce;
			var rigidList = new List<Rigidbody2D>();

			foreach (var pool in ballPhysicBools)
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
					rb.AddForce(newShakeForce, ForceMode2D.Impulse);
				}
			}
		}

		public Fruit2D GetLastBallFromPrefabs()
		{
			return ballNoPhysicPrefabs[^1];
		}

		public bool BilliardThemeIndex { get; private set; }
	}
}
