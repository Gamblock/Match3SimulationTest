using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Config
{
	public class GameConfig : ScriptableObject
	{
		 [SerializeField]
		 private BoardConfig[] boardConfigs;

		 public BoardConfig[] BoardConfigs => boardConfigs;
	}
}