
using GamePlay.Core.Board;

namespace GamePlay.Core.BoardProcessorEvents
{
    public struct FallEvent
    {
        public readonly BoardPosition from;
        public readonly int depth;

        public FallEvent(BoardPosition p, int d)
        {
            from = p;
            depth = d;
        }
        public override string ToString()
        {
            return "{\"@event\": \"fall\", \"from\": " + from.ToString() + ", \"depth\":" + depth.ToString() + "}";
        }
    }
}