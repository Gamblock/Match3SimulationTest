using Features.Signals;
using Features.Views;
using GamePlay.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GamePlay.Presenters
{
    public class SelectConfigPresenter : MonoBehaviour
    {
        [SerializeField]
        public GameObject buttonLevelSelectPrefab;

        [SerializeField]
        public GridLayoutGroup gridLayout;

        private SignalBus signalBus;
        private GameModel gameModel;

        [Inject]
        public void Inject(SignalBus signalBus, GameModel gameModel)
        {
            this.gameModel = gameModel;
            this.signalBus = signalBus;

            InitButtons();
        }

        private void InitButtons()
        {
            for (int i = 0; i < gameModel.LevelsAmount; i++)
            {
                var levelIndex = i;
                var gameObject = Instantiate(buttonLevelSelectPrefab, gridLayout.transform);
                var button = gameObject.GetComponent<LevelChooseButton>();
                button.SetLevelIndex(levelIndex);
                button.OnClick += StartLevel;
            }
        }

        private void StartLevel(int level)
        {
            gameModel.CurrentLevelIndex = level;
            signalBus.Fire(new ChangeLevelSignal(level));
        }
    }
}