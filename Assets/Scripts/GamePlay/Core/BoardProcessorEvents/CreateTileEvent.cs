using GamePlay.Core.Board;
namespace GamePlay.Core.BoardProcessorEvents
{
    public struct CreateTileEvent
    {
        public readonly BoardPosition Position;
        public readonly int Color;

        public CreateTileEvent(BoardPosition position, int color)
        {
            Position = position;
            Color = color;
        }

        public override string ToString()
        {
            return "{\"@event\": \"createTile\", \"position\": " + Position.ToString() + ", \"color\":" +
                   Color.ToString() + "}";
        }
    }
}