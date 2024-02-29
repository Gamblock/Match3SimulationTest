using System.Collections.Generic;
using GamePlay.Core.BoardProcessorEvents;
using Newtonsoft.Json;

namespace Match3
{
    public struct SwapResponse
    {
        public readonly List<MatchFallStep> steps;
        public readonly List<CreateTileEvent> fills;

        public SwapResponse(List<MatchFallStep> s, List<CreateTileEvent> f)
        {
            steps = s;
            fills = f;
        }
    }
}