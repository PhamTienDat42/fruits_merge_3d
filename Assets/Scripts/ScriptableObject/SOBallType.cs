using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "BallTypes", menuName = "Ball/BallTypes")]
	public class SOBallType : ScriptableObject
	{
		[SerializeField] private List<BallType> ballTypes = new();

		public List<BallType> BallTypes { get => ballTypes; set => ballTypes = value; }
	}

	[System.Serializable]
	public struct BallType
	{
		public string Name;
		public List<Fruits.Fruit2D> Fruit2D;
	}
}
