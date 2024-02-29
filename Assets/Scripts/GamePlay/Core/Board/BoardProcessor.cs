using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay.Core.Board;
using GamePlay.Core.BoardProcessorEvents;
using Random = System.Random;
namespace Match3
{
    /// <summary>
    /// Class that calculates board states, it does not use any Unity classes so it can be tested outside of Unity context
    /// GameSimulator class can be used to run random swaps and save logs to file
    /// </summary>
    public class BoardProcessor
    {
        private readonly int[,] _POS_DIFF = { { 0, -1 }, { +1, 0 }, { -1, 0 },{ 0, +1}};
        private readonly int _COLOR_NULL = 0;

        private readonly int columnsCount;
        private readonly int rowsCount;
        private readonly int colorsCount;

        private readonly List<List<int>> colors;
        private readonly Random random;
        
        private bool initialised;
        

        public BoardProcessor(int columnsCount_, int rowsCount_, int colorsCount_, int seed = 0)
        {
            if (CheckBoardArgs(columnsCount_, rowsCount_, colorsCount_))
            {
                initialised = false;

                rowsCount = rowsCount_;
                columnsCount = columnsCount_;
                colorsCount = colorsCount_;

                colors = new List<List<int>>();
                for (int col = 0; col != columnsCount; ++col)
                {
                    colors.Add(new List<int>());
                    for (int row = 0; row != rowsCount; ++row)
                    {
                        colors[col].Add(_COLOR_NULL);
                    }
                }
                random = new Random(seed);
            }
            else
            {
                throw new InvalidBoardArgsException();
            }
        }
        
        public List<CreateTileEvent> GetNewBoard()
        {
            initialised = true;
            return FillForBoardPositions(GetAllNullColoredPositions());
        }

        
        public SwapResponse Swap(BoardPosition a, BoardPosition b)
        {
            if (!initialised)
            {
                throw new BoardUninitializedException();
            }
            if (
                !CheckBordersForPosition(a) ||
                !CheckBordersForPosition(b) ||
                !CheckNeighbours(a, b)
            )
            {
                throw new InvalidSwapArgsException();
            }

            (
                colors[a.col][a.row],
                colors[b.col][b.row]
            ) = (
                colors[b.col][b.row],
                colors[a.col][a.row]
            );

            var steps = new List<MatchFallStep>();
            
            var matches = GetMatchesForPositions(new List<BoardPosition>() { a, b });
           
            while (matches.Count != 0)
            {
                var destroys = DestroysFromBoardPositions(matches);
                var falls = FallForBoardPositions(matches);
                var toCheck = new List<BoardPosition>();
                foreach (FallEvent fallEvent in falls)
                {
                    var newPos = new BoardPosition(fallEvent.from.col, fallEvent.from.row - fallEvent.depth);
                    toCheck.Add(newPos);
                }
                steps.Add(new MatchFallStep(destroys, falls));
                matches = GetMatchesForPositions(toCheck);
            }

            var fills = FillForBoardPositions(GetAllNullColoredPositions());
            
            return new SwapResponse(steps, fills);
        }
        
        
        public void PrintBoard()
        {
            for (int i = rowsCount - 1; i != -1; --i)
            {
                var row = new List<int>();
                for (int j = 0; j != columnsCount; ++j)
                {
                    row.Add(colors[j][i]);
                }
                string rowText = string.Join(".", row.ToArray());
                Console.WriteLine(rowText);
            }
        }
        
        private static bool CheckNeighbours(BoardPosition a, BoardPosition b)
        {
            return (
                ((Math.Abs(a.col - b.col) == 1) && (a.row == b.row)) ||
                ((Math.Abs(a.row - b.row) == 1) && (a.col == b.col))
            );
        }

        private List<BoardPosition> GetAllNullColoredPositions()
        {
            List<BoardPosition> result = new List<BoardPosition>();

            for (int row = 0; row != rowsCount; ++row)
            {
                for (int col = 0; col != columnsCount; ++col)
                {
                    BoardPosition position = new BoardPosition(col, row);
                    if (GetColorForPosition(position) == _COLOR_NULL)
                    {
                        result.Add(position);
                    }
                }
            }

            return result;
        }

        private bool CheckBordersForPosition(BoardPosition p)
        {
            return (
                p.col >= 0 &&
                p.col < columnsCount &&
                p.row >= 0 &&
                p.row < rowsCount
            );
        }

        private int GetColorForPosition(BoardPosition p)
        {
            if (CheckBordersForPosition(p))
            {
                return colors[p.col][p.row];
            }
            return _COLOR_NULL;
        }

        private List<BoardPosition> GetMatchesForPosition(BoardPosition boardPosition)
        {
            List<BoardPosition> matchedPositions = new List<BoardPosition>();
            int color = GetColorForPosition(boardPosition);

            if (color == _COLOR_NULL)
            {
                return matchedPositions;
            }

            matchedPositions = CheckNeighbours(color, boardPosition);

            var count = matchedPositions.Count;
            
            if (count != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    matchedPositions.AddRange(CheckNeighbours(color,matchedPositions[i]));
                }
                
                matchedPositions.Add(boardPosition);
            }
            
            return matchedPositions;
        }

        private List<BoardPosition> CheckNeighbours(int color, BoardPosition position)
        {
            var matches = new List<BoardPosition>(); 
            
            for (int i = 0; i != 4; ++i)
            {
                 var n1 = BoardPosition.Add(position, _POS_DIFF[i, 0], _POS_DIFF[i, 1]);
                 var n2 = BoardPosition.Add(n1, _POS_DIFF[i, 0], _POS_DIFF[i, 1]);
                 var n3 = BoardPosition.Add(n1, _POS_DIFF[3 - i, 0], _POS_DIFF[i, 1]);
                
                if (color == GetColorForPosition(n1) && color == GetColorForPosition(n2))
                {
                    matches.Add(n1);
                    matches.Add(n2);
                }

                if (color == GetColorForPosition(n1) && color == GetColorForPosition(n3))
                {
                    matches.Add(n1);
                    matches.Add(n3);
                }
            }

            return matches;
        }
        
        private List<BoardPosition> GetMatchesForPositions(List<BoardPosition> positions)
        {
            HashSet<BoardPosition> matchesSet = new HashSet<BoardPosition>();
            foreach (BoardPosition p in positions)
            {
                foreach (BoardPosition m in GetMatchesForPosition(p))
                {
                    matchesSet.Add(m);
                }
            }
            return matchesSet.ToList();
        }

        private List<List<int>> PositionsToColumnHistogram(List<BoardPosition> positions)
        {
            List<List<int>> histogram = new List<List<int>>();
            for (int i = 0; i != columnsCount; ++i)
            {
                histogram.Add(new List<int>());
            }

            foreach (BoardPosition p in positions)
            {
                histogram[p.col].Add(p.row);
            }
            foreach (List<int> histogramColumn in histogram)
            {
                histogramColumn.Sort();
            }
            return histogram;
        }

        private List<List<int>> PositionsToRowHistogram(List<BoardPosition> positions)
        {
            List<List<int>> histogram = new List<List<int>>();
            for (int i = 0; i != rowsCount; ++i)
            {
                histogram.Add(new List<int>());
            }

            foreach (BoardPosition p in positions)
            {
                histogram[p.row].Add(p.col);
            }
            foreach (List<int> histogramRow in histogram)
            {
                histogramRow.Sort();
            }

            return histogram;
        }

        private List<DestroyEvent> DestroysFromBoardPositions(List<BoardPosition> matchedPositions)
        {
            List<DestroyEvent> destroys = new List<DestroyEvent>();

            foreach (BoardPosition p in matchedPositions)
            {
                destroys.Add(new DestroyEvent(p));
            }

            return destroys;
        }

        private List<CreateTileEvent> FillForBoardPositions(List<BoardPosition> matchedPositions)
        {
            List<CreateTileEvent> fills = new List<CreateTileEvent>();
            List<List<int>> hist = PositionsToRowHistogram(matchedPositions);

            for (int row = 0; row != rowsCount; ++row)
            {
                for (int ptr = 0; ptr != hist[row].Count; ++ptr)
                {
                    int col = hist[row][ptr];
                    BoardPosition p = new BoardPosition(col, row);
                    bool isFilled = false;
                    foreach (int color in GenRandomPermutation(colorsCount))
                    {
                        if (!isFilled && !CheckMatchesForPositionColor(p, color))
                        {
                            fills.Add(new CreateTileEvent(p, color));
                            colors[p.col][p.row] = color;
                            isFilled = true;
                        }
                    }
                }
            }

            return fills;
        }

        private List<int> GenRandomPermutation(int n)
        {
            List<int> result = new List<int>();
            for (int i = 1; i <= n; i++)
            {
                result.Add(i);
            }

            for (int i = 0; i != n; ++i)
            {
                int j = random.Next(i,n);
                (result[i], result[j]) = (result[j], result[i]);
            }
            return result;
        }

        private bool CheckMatchesForPositionColor(BoardPosition p, int color)
        {
            if (color == _COLOR_NULL)
            {
                return false;
            }

            for (int i = 0; i != 4; ++i)
            {
                BoardPosition n1 = BoardPosition.Add(p, _POS_DIFF[i, 0], _POS_DIFF[i, 1]);
                BoardPosition n2 = BoardPosition.Add(n1, _POS_DIFF[i, 0], _POS_DIFF[i, 1]);
                if (color == GetColorForPosition(n1) && color == GetColorForPosition(n2))
                {
                    return true;
                }
            }
            return false;
        }

        private List<FallEvent> FallForBoardPositions(List<BoardPosition> matchedPositions)
        {
            var falls = new List<FallEvent>();
            var hist = PositionsToColumnHistogram(matchedPositions);

            for (int col = 0; col != columnsCount; ++col)
            {
                var colorsCol = new List<int>();
                int row = 0;
                hist[col].Add(rowsCount);

                for (int ptr = 0; ptr != hist[col].Count;)
                {
                    for (; row < hist[col][ptr]; ++row)
                    {
                        int colorToAdd = colors[col][row];
                        if (colorToAdd != _COLOR_NULL)
                        {
                            colorsCol.Add(colorToAdd);
                            if (ptr != 0)
                            {
                                falls.Add(new FallEvent(new BoardPosition(col, row), ptr));
                            }
                        }
                    }
                    row = hist[col][ptr] + 1;
                    ++ptr;
                }
                while (colorsCol.Count < rowsCount)
                {
                    colorsCol.Add(0);
                }
                colors[col] = colorsCol.ToList();
            }

            return falls;
        }

        private bool CheckBoardArgs(int columnsCount_, int rowsCount_, int colorsCount_)
        {
            return (
                rowsCount_ > 0 &&
                columnsCount_ > 0 &&
                rowsCount_ + columnsCount_ > 2 &&
                colorsCount_ > 2
            );
        }
    }

    public class BoardUninitializedException : Exception
    {
    }

    public class InvalidSwapArgsException : Exception
    {
    }

    public class InvalidBoardArgsException : Exception
    {
    }
}