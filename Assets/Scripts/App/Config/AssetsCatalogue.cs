using Features.Config;
using Features.Data;
using UnityEngine;

namespace App.Config
{
  
    [CreateAssetMenu(fileName = "New AssetsCatalogue", menuName = "AssetCatalogue/AssetsCatalogue")]
    public class AssetsCatalogue : ScriptableObject
    {
        [SerializeField]
         private TileSpriteConfig[] tileSpriteConfigs;

        public Sprite GetSpriteConfig(TileColor tileColor)
        {
            foreach (var spriteConfig in tileSpriteConfigs)
            {
                if (spriteConfig.Color == tileColor)
                    return spriteConfig.Sprite;
            }

            return null;
        }
    }
}