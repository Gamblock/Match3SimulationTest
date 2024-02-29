using System.Collections.Generic;
using Match3;

namespace GamePlay.Core.BoardProcessorEvents
{
    public struct MatchFallStep
    {
        public readonly List<DestroyEvent> destroyed;
        public readonly List<FallEvent> fell;

        public MatchFallStep(List<DestroyEvent> d, List<FallEvent> f)
        {
            destroyed = d;
            fell = f;
        }
    }
}