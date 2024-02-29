using Unity.Mathematics;

namespace Features.Signals
{
    public struct TileMoveData
    {
        public readonly int2 StartingPosition;
        public readonly int2 TargetPosition;

        public TileMoveData(int2 startingPosition, int2 targetPosition)
        {
            StartingPosition = startingPosition;
            TargetPosition = targetPosition;
        }
    }
}