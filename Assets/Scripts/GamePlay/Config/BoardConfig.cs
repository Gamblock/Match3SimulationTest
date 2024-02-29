using System.Collections.Generic;
using Features.Data;
using UnityEngine;

namespace Features.Config
{
	public class BoardConfig : ScriptableObject
	{
		[SerializeField] 
		private int width;
		public int Width => width;

		[SerializeField] 
		private int height;
		public int Height => height;

		[SerializeField] 
		private bool useRandomSeed;
		public bool UseRandomSeed => useRandomSeed;

		[SerializeField] 
		private List<TileColor> availableColors;
		public List<TileColor> AvailableColors => availableColors;
	}
}