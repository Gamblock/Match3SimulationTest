using System.Collections.Generic;
using Features.Data;
using Features.Signals;
using GamePlay.Core.Board;
using GamePlay.Core.BoardProcessorEvents;
using GamePlay.Models;
using GamePlay.Signals;
using JetBrains.Annotations;
using Match3;
using Unity.Mathematics;
using Zenject;

namespace Features.Commands
{
   
    [UsedImplicitly]
    public class BoardModelHandler
    {
        [Inject] 
        private SignalBus signalBus;
	
        [Inject] 
        private GameModel gameModel;
        
        
        private BoardProcessor boardProcessor;
      
	
        public void CreateBoard(Match3Signals.CreateBoardSignal createBoardSignal)
        {
            var levelConfig = gameModel.GetCurrentLevelConfig();
            levelConfig = gameModel.GetCurrentLevelConfig();
            boardProcessor = levelConfig.UseRandomSeed ? 
                new BoardProcessor(levelConfig.Width, levelConfig.Height, levelConfig.AvailableColors.Count, UnityEngine.Random.Range(0,1000)) :
                new BoardProcessor(levelConfig.Width, levelConfig.Height, levelConfig.AvailableColors.Count);
            
            BoardCalculator.InitBoardSize(levelConfig.Width, levelConfig.Height);
            signalBus.Fire(new Match3Signals.TileCreatedSignal(CreateTilesData(boardProcessor.GetNewBoard())));
            signalBus.Fire<Match3Signals.OnBoardCreatedSignal>();
        }

        public void HandleSwap(Match3Signals.CalculateMatchesSignal swapSignal)
        {
            var pos1 = new BoardPosition((int)swapSignal.FirstSwapPosition.x, (int)swapSignal.FirstSwapPosition.y);
            var pos2 = new BoardPosition((int)swapSignal.SecondSwapPosition.x, (int)swapSignal.SecondSwapPosition.y);
            var swapResponse = boardProcessor.Swap(pos1, pos2);
            HandleSteps(swapResponse);
        }
        
        private void HandleSteps(SwapResponse swapResponse)
        {
            var matchStepEvents = swapResponse.steps;
            var matchSteps = new List<Match3Signals.MatchStep>();
            
            foreach (var matchStep in matchStepEvents)
            {
                var tilesMoveData = new List<TileMoveData>();
                List<int2> destroyedTiles = new List<int2>();
                
                foreach (var destroy in matchStep.destroyed)
                {
                    destroyedTiles.Add(destroy.position.GetPositionInt2());
                }
                
                foreach (var fallEvent in matchStep.fell)
                {
                    var from = fallEvent.from.GetPositionInt2();
                    var to = new int2(from.x, from.y - fallEvent.depth);
                   tilesMoveData.Add(new TileMoveData(from,to));
                }
                matchSteps.Add(new Match3Signals.MatchStep(tilesMoveData,destroyedTiles));
            }
            
            signalBus.Fire(new Match3Signals.SwapResultSignal(CreateTilesData(swapResponse.fills),matchSteps));
        }
        
        private List<Match3Signals.TileCreateData> CreateTilesData(List<CreateTileEvent> tilesToCreate)
        {
            var levelConfig = gameModel.GetCurrentLevelConfig();
            var availableColors = levelConfig.AvailableColors;
            var tilesCreateData = new List<Match3Signals.TileCreateData>();
            foreach (var tile in tilesToCreate)
            {
                tilesCreateData.Add(new Match3Signals.TileCreateData(availableColors[tile.Color - 1],
                    tile.Position.GetPositionInt2()));
            }

            return tilesCreateData;
        }
    }
}