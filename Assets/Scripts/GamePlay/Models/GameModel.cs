using Features.Config;
using Features.Signals;
using JetBrains.Annotations;
using Zenject;

namespace GamePlay.Models
{
    
    [UsedImplicitly]
    public class GameModel
    {
        [Inject]
        private GameConfig gameConfig;
		
        [Inject]
        private SignalBus signalBus;
        
        private int currentConfigIndex;
        
        public int CurrentLevelIndex
        {
            get => currentConfigIndex;
            set
            {
                if(value < 0 || value >= gameConfig.BoardConfigs.Length)
                    return;
				
                currentConfigIndex = value;
                signalBus.Fire(new ConfigChangedSignal(value));
            }
        }

        public int LevelsAmount => gameConfig.BoardConfigs.Length;

        public BoardConfig GetCurrentLevelConfig()
        {
            return gameConfig.BoardConfigs[CurrentLevelIndex];
        }
    }
}