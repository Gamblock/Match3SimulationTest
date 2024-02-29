using App.Config;
using Features.Config;
using UnityEngine;
using Zenject;

namespace App.Installers
{
    /// <summary>
    /// Installer with SO dependencies (mostly Game Design configs)
    /// </summary>
    [CreateAssetMenu(fileName = "SOInstaller", menuName = "Installers/SOInstaller")]
    public class ScriptableObjectInstaller : ScriptableObjectInstaller<ScriptableObjectInstaller>
    {
        [Header("Art Assets configuration")] 
        public GameSceneCatalogue GameSceneCatalogue;
        public AssetsCatalogue AssetsCatalogue;

       [Header("Game data configuration")] 
        public GameConfig BoardConfig;
    
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameSceneCatalogue>().FromInstance(GameSceneCatalogue).AsSingle();
            Container.BindInterfacesAndSelfTo<AssetsCatalogue>().FromInstance(AssetsCatalogue).AsSingle();
            Container.BindInterfacesAndSelfTo<GameConfig>().FromInstance(BoardConfig).AsSingle();
        }
    }
}