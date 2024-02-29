using GamePlay.Core.Board;

namespace GamePlay.Core.BoardProcessorEvents
{
    public struct DestroyEvent
    {
        public readonly BoardPosition position;

        public DestroyEvent(BoardPosition p)
        {
            position = p;
        }

        public override string ToString()
        {
            return "{\"@event\": \"destroy\", \"position\": " + position.ToString() + "}";
        }
    }
}