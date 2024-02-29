using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.Config
{
    /// <summary>
    /// References to addressable scenes
    /// </summary>
    [CreateAssetMenu(fileName = "New GameSceneCatalogue", menuName = "AssetCatalogue/GameSceneCatalogue")]
    public class GameSceneCatalogue : ScriptableObject
    {
        public AssetReference MapScene;
        public AssetReference BoardScene;
    }
}