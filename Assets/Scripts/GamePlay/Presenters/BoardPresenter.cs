using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Config;
using Core.Utils;
using DG.Tweening;
using Features.Data;
using Features.Signals;
using Features.Views;
using GamePlay.Signals;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace GamePlay.Presenters
{
    public class BoardPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject gemPrefab;
        [SerializeField] private GameObject gemsContainer;
        [SerializeField] private ParticleEffectView spawnParticlesPrefab;
        [SerializeField] private float swapDuration = 0.25f;
        [SerializeField] private float moveDuration = 0.25f;

        private readonly Dictionary<int2, TileView> tileViews = new();
        private Pool<TileView> tilePool;
        private SignalBus signalBus;
        private AssetsCatalogue assetsCatalogue;

        private float boardScale;


        [Inject]
        public void Inject(SignalBus signalBus, DiContainer diContainer,
            AssetsCatalogue assetsCatalogue)
        {
            this.signalBus = signalBus;
            this.assetsCatalogue = assetsCatalogue;

            tilePool = new Pool<TileView>(diContainer, gemPrefab, gemsContainer.transform, 100);
            gemPrefab.SetActive(false);
        }

        private void OnEnable()
        {
            signalBus.Subscribe<Match3Signals.TileCreatedSignal>(OnTilesCreated);
            signalBus.Subscribe<Match3Signals.SwapResultSignal>(OnBoardStateCalculated);
            signalBus.Subscribe<Match3Signals.OnBoardCreatedSignal>(OnBoardCreated);
            signalBus.Subscribe<Match3Signals.StartSwapSignal>(OnSwapTilesAsync);
        }

        private void OnDisable()
        {
            signalBus.Unsubscribe<Match3Signals.TileCreatedSignal>(OnTilesCreated);
            signalBus.Unsubscribe<Match3Signals.SwapResultSignal>(OnBoardStateCalculated);
            signalBus.Unsubscribe<Match3Signals.OnBoardCreatedSignal>(OnBoardCreated);
            signalBus.Unsubscribe<Match3Signals.StartSwapSignal>(OnSwapTilesAsync);

            Dispose();
        }

        private void OnDestroy()
        {
            tilePool.Clear();
        }

        private void OnBoardCreated()
        {
            var scaleX = 8 / BoardCalculator.GetBoardWidthSize();
            var scaleY = 11 / BoardCalculator.GetBoardHeightSize();
            boardScale = Mathf.Min(scaleX, scaleY);
            gemsContainer.transform.localScale = new Vector3(boardScale, boardScale, 1);
        }

        private void EnableInput()
        {
            foreach (KeyValuePair<int2, TileView> keyValuePair in tileViews)
            {
                keyValuePair.Value.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        private void DisableInput()
        {
            foreach (KeyValuePair<int2, TileView> keyValuePair in tileViews)
            {
                keyValuePair.Value.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private void OnTilesCreated(Match3Signals.TileCreatedSignal signal)
        {
            foreach (var tileData in signal.CreateTilesSignal)
            {
                HandleTileCreation(tileData.Color, tileData.BoardPosition);
            }
        }

        private void FillTiles(List<Match3Signals.TileCreateData> fillTilesData)
        {
            foreach (var createTileData in fillTilesData)
            {
                var spawnPosition = new int2(createTileData.BoardPosition.x, BoardCalculator.GetBoardHeight());
                HandleTileCreation(createTileData.Color, spawnPosition);
                MoveTile(spawnPosition, createTileData.BoardPosition);
            }
        }

        private void HandleTileCreation(TileColor color, int2 boardPosition)
        {
            var tileView = tilePool.Rent();
            tileView.Init(boardPosition, color);
            tileView.transform.localPosition = BoardCalculator.ConvertBoardPositionToTransformPosition(boardPosition);
            tileViews.Add(boardPosition, tileView);
            tileView.OnDestructionAnimationComplete += OnDestructionAnimationComplete;
        }

        private async void OnBoardStateCalculated(Match3Signals.SwapResultSignal signal)
        {
            foreach (var matchStep in signal.MatchSteps)
            {
                foreach (var destroyedTile in matchStep.DestroyedTiles)
                {
                    var tileView = tileViews[destroyedTile];
                    HandleTileDestruction(tileView);
                    tileViews.Remove(destroyedTile);
                }

                foreach (var movedTile in matchStep.TilesMoveData)
                {
                    MoveTile(movedTile.StartingPosition, movedTile.TargetPosition);
                }

                await Task.Delay(TimeSpan.FromSeconds(moveDuration));
            }

            FillTiles(signal.CreateTilesData);
            await Task.Delay(TimeSpan.FromSeconds(moveDuration));

            EnableInput();
        }
        
        private void HandleTileDestruction(TileView tileView)
        {
            var effectView = Instantiate(spawnParticlesPrefab, transform);
            effectView.Show(assetsCatalogue.GetSpriteConfig(tileView.GetTileColor()));

            Vector3 position = tileView.transform.position;
            position.z = -5;
            effectView.transform.position = position;
            var originalScale = gameObject.transform.localScale;
            effectView.transform.localScale = new Vector3(originalScale.x * boardScale, originalScale.y * boardScale,
                originalScale.z);

            tileView.PlayDestroyAnimation();
        }

        private async void OnSwapTilesAsync(Match3Signals.StartSwapSignal signal)
        {
            DisableInput();
            var view1 = tileViews[signal.FirstSwapPosition];
            var view2 = tileViews[signal.SecondSwapPosition];

            view1.SetPosition(signal.SecondSwapPosition);
            view2.SetPosition(signal.FirstSwapPosition);

            tileViews[signal.FirstSwapPosition] = view2;
            tileViews[signal.SecondSwapPosition] = view1;
            SwapTiles(view1, view2);

            await Task.Delay(TimeSpan.FromSeconds(swapDuration));
            signalBus.Fire(
                new Match3Signals.CalculateMatchesSignal(signal.FirstSwapPosition, signal.SecondSwapPosition));
        }

        private void SwapTiles(TileView tile1, TileView tile2)
        {
            DOTween.Sequence()
                .Join(tile1.transform.DOMove(tile2.transform.position, swapDuration))
                .Join(tile2.transform.DOMove(tile1.transform.position, swapDuration))
                .SetEase(Ease.Flash);
        }

        private void MoveTile(int2 startingBoardPosition, int2 targetBoardPosition)
        {
            var tile = tileViews[startingBoardPosition];
            var targetPosition = BoardCalculator.ConvertBoardPositionToTransformPosition(targetBoardPosition);
            tile.SetPosition(targetBoardPosition);
            tileViews.Add(targetBoardPosition, tile);
            tileViews.Remove(startingBoardPosition);

            DOTween.Sequence().Join(tile.transform.DOLocalMove(targetPosition, moveDuration)).SetEase(Ease.Flash);
        }

        private void OnDestructionAnimationComplete(TileView view)
        {
            view.OnDestructionAnimationComplete -= OnDestructionAnimationComplete;
            tilePool.Return(view);
        }

        private void Dispose()
        {
            tileViews.Clear();
            tilePool.ReturnAll();
        }
    }
}