using System;
using Unity.Mathematics;

namespace GamePlay.Core.Board
{
    public struct BoardPosition
    {
        public readonly int col;
        public readonly int row;

        public BoardPosition(int col_, int row_)
        {
            col = col_;
            row = row_;
        }

        public static BoardPosition Add(BoardPosition x, int dcol, int drow)
        {
            return new BoardPosition(x.col + dcol, x.row + drow);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is BoardPosition)
            {
                BoardPosition other = (BoardPosition)obj;
                return col == other.col && row == other.row;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return new Tuple<int, int>(col, row).GetHashCode();
        }

        public override string ToString()
        {
            return "{\"col\":" + this.col.ToString() + ", \"row\": " + this.row.ToString() + "}";
        }

        public int2 GetPositionInt2()
        {
            return new int2(col, row);
        }
    }
}