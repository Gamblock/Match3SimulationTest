using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Data
{
    /// <summary>
    /// Basic math operations for the board logic
    /// </summary>
    public static class BoardCalculator
    {
        private const float TILE_SIZE = 1.3f;
        
        private static int boardWidth;
        private static int boardHeight;
        private static float startX;
        private static float startY;

        public static void InitBoardSize(int boardWidth, int boardHeight)
        {
            BoardCalculator.boardWidth = boardWidth;
            BoardCalculator.boardHeight = boardHeight;
            startX = - ((float) boardWidth / 2) * TILE_SIZE;
            startY = - ((float) boardHeight / 2) * TILE_SIZE;
        }
        
        public static Vector3 ConvertBoardPositionToTransformPosition(int2 boardPosition)
        {
            return new Vector3(startX + boardPosition.x*TILE_SIZE + TILE_SIZE/2, startY + boardPosition.y*TILE_SIZE + TILE_SIZE/2, 0);
        }

        public static bool IsNextToEachOther(int2 gem1Position, int2 gem2Position)
        {
            if (gem1Position.x == gem2Position.x)
            {
                return Mathf.Abs(gem1Position.y - gem2Position.y) == 1;
            }
            if (gem1Position.y == gem2Position.y)
            {
                return Mathf.Abs(gem1Position.x - gem2Position.x) == 1;
            }

            return false;
        }
        
        public static int GetBoardWidth()
        {
            return boardWidth;
        }

        public static int GetBoardHeight()
        {
            return boardHeight;
        }
        
        public static float GetBoardWidthSize()
        {
            return TILE_SIZE * boardWidth;
        } 
        
        public static float GetBoardHeightSize()
        {
            return TILE_SIZE * boardHeight;
        }
    }
}