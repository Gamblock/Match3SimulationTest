using System.Collections.Generic;
using Features.Data;
using Features.Signals;
using Unity.Mathematics;

namespace GamePlay.Signals
{
    public class Match3Signals
    {
        public class CreateBoardSignal
        {
        }
        
        public class OnBoardCreatedSignal
        {
        } 
        
        public class FindMatchesSignal
        {
        }
        
        public class OnPlayerTurnStart
        {
        } 
        
        public class StartSwapSignal
        {
            public readonly int2 FirstSwapPosition;
            public readonly int2 SecondSwapPosition;

            public StartSwapSignal(int2 firstSwapPosition, int2 secondSwapPosition)
            {
                FirstSwapPosition = firstSwapPosition;
                SecondSwapPosition = secondSwapPosition;
            }
        }

        public class CalculateMatchesSignal
        {
            public readonly int2 FirstSwapPosition;
            public readonly int2 SecondSwapPosition;

            public CalculateMatchesSignal(int2 firstSwapPosition, int2 secondSwapPosition)
            {
                FirstSwapPosition = firstSwapPosition;
                SecondSwapPosition = secondSwapPosition;
            }
        }
        
        public class PlayerScoreChangedSignal
        {
        }
        
        public class TurnAmountChangedSignal
        {
        }
        
        public class OutOfTurnsSignal
        {
        }
        
        public class TileCreatedSignal
        {
            public readonly List<TileCreateData> CreateTilesSignal;

            public TileCreatedSignal(List<TileCreateData> createTilesSignal)
            {
                CreateTilesSignal = createTilesSignal;
            }
        }
        
        public struct TileCreateData
        {
            public readonly TileColor Color;
            public readonly int2 BoardPosition;

            public TileCreateData(TileColor color, int2 boardPosition)
            {
                Color = color;
                BoardPosition = boardPosition;
            }
        }
        public struct MatchStep
        {
            public readonly List<TileMoveData> TilesMoveData;
            public readonly List<int2> DestroyedTiles;

            public MatchStep(List<TileMoveData> tilesMoveData, List<int2> destroyedTiles )
            {
                TilesMoveData = tilesMoveData;
                DestroyedTiles = destroyedTiles;
            }
        }
        public class SwapResultSignal
        {
           
            public readonly List<TileCreateData> CreateTilesData;
            public readonly List<MatchStep> MatchSteps;

            public SwapResultSignal(List<TileCreateData> createTilesData, List<MatchStep> matchSteps)
            {
                CreateTilesData = createTilesData;
                MatchSteps = matchSteps;
            }
        }
    }
}