using Game;
using Pools;
using UnityEngine;

namespace Fruits
{
	public class Fruit : MonoBehaviour
	{
		private GameController controller;
		private FruitPools fruitPools;
		private GameView gameView;

		public void InstantiateFruits(FruitPools fruitPools, GameController gameController, GameView gameView)
		{
			this.fruitPools = fruitPools;
			this.controller = gameController;
			this.gameView = gameView;
		}

	}
}
