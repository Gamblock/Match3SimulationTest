using System.Collections.Generic;
using System.IO;
using GamePlay.Core.Board;
using GamePlay.Core.BoardProcessorEvents;
using Match3;
using Newtonsoft.Json;
using Random = System.Random;

namespace GamePlay.Core
{
    /// <summary>
    /// Class made to simulate an arbitrary amount of moves on a board and save logs to file
    /// </summary>
    public class GameSimulator
    {
        public struct SwapMove
        {
            private readonly BoardPosition from;
            private readonly BoardPosition to;

            public SwapMove(BoardPosition from_, BoardPosition to_)
            {
                from = from_;
                to = to_;
            }

            public override string ToString()
            {
                return ",\n{\"@event\": \"swap\", \"from\":" + from + ", \"to\": " + to + "}";
            }
        }
        
        public struct RunResult
        {
            private readonly List<CreateTileEvent> initialFills;
            private readonly List<SwapMove> swapMoves;
            private readonly List<SwapResponse> swapReport;

            public RunResult(List<CreateTileEvent> _0, List<SwapMove> _1, List<SwapResponse> _2)
            {
                initialFills = _0;
                swapMoves = _1;
                swapReport = _2;
            }

            public override string ToString()
            {
                string first = "[" + string.Join(",\n", initialFills);
                string second = "";
                for (int i = 0; i < swapMoves.Count; i++)
                {
                    second += ",\n" + swapMoves[i].ToString();
                    foreach (MatchFallStep step in swapReport[i].steps)
                    {
                        second += string.Join(",\n", step.destroyed);
                        second += ",\n";
                        second += string.Join(",\n", step.fell);
                        second += ",\n";
                    }

                    second += string.Join(",\n", swapReport[i].fills);
                }

                string third = "]\n";
                return first + second + third;
            }
        }

        private readonly int[,] _POS_DIFF = { { 0, -1 }, { +1, 0 }, { 0, +1 }, { -1, 0 } };

        private BoardProcessor game;


        public RunResult RunRandomSwaps(uint steps, int cols, int rows, int colors, int randomSeed = 0)
        {
            game = new BoardProcessor(cols, rows, colors, 0);

            List<CreateTileEvent> firstFills = game.GetNewBoard();
            List<SwapMove> swapMoves = new List<SwapMove>();
            List<SwapResponse> swapReports = new List<SwapResponse>();

            int popped = 0;
            for (int i = 0; i != steps; ++i)
            {
                var rnd = new Random(randomSeed);

                int col = UnityEngine.Random.Range(1, cols - 1);
                int row = UnityEngine.Random.Range(1, rows - 1);
                int dptr = UnityEngine.Random.Range(0, 4);
                BoardPosition pos1 = new BoardPosition(col, row);
                BoardPosition pos2 = new BoardPosition(col + _POS_DIFF[dptr, 0], row + _POS_DIFF[dptr, 1]);
                SwapResponse response = game.Swap(pos1, pos2);
                swapMoves.Add(new SwapMove(pos1, pos2));
                swapReports.Add(response);
                popped += response.fills.Count;
            }

            return new RunResult(firstFills, swapMoves, swapReports);
        }

        public void LogRunResults(RunResult result, string filename)
        {
            if (filename != null)
            {
                File.WriteAllText(filename, result.ToString());
            }
        }

        public void LogRunResultsJson(RunResult result, string filename)
        {
            var resultJson = JsonConvert.SerializeObject(result);

            if (filename != null)
            {
                File.WriteAllText(filename, resultJson);
            }
        }
    }
}