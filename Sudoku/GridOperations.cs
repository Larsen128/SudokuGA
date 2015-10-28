using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class GridOperations
    {
        private int _n;

        public GridOperations(int n)
        {
            _n = n;
        }

        public int N
        {
            get { return _n; }
            set { _n = value; }
        }

        public int N2
        {
            get { return _n * _n; }
        }

        public event EventHandler<SudokuPacketEventArgs> CommonOperations;

        public void GetGridScore(int[,] grid, int[,] grid1 = null)
        {
            if (grid.Length != N2 * N2)
            {
                throw new InvalidOperationException("La dimension de la grille est invalide");
            }

            int[] cells = new int[N2];
            int[] linesX = new int[N2];
            int[] linesY = new int[N2];

            int[] cells1 = new int[N2];
            int[] linesX1 = new int[N2];
            int[] linesY1 = new int[N2];

            int[,] smallGrid = new int[N, N];
            int[,] smallGrid1 = new int[N, N];

            int ni = 0;
            int nj = 0;

            for (int i = 0; i < N2; i++)
            {
                if (ni + 1 > N2)
                {
                    ni = 0;
                }

                for (int j = 0; j < N2; j++)
                {
                    int mi = j % N;
                    int mj = (j - mi) / N;

                    cells[j] = grid[ni + mi, nj + mj];
                    cells1[j] = grid1 == null ? 0 : grid1[ni + mi, nj + mj];
                    smallGrid[mi, mj] = grid[ni + mi, nj + mj];
                    smallGrid1[mi, mj] = grid1 == null ? 0 : grid1[ni + mi, nj + mj];

                    linesX[j] = grid[i, j];
                    linesX1[j] = grid1 == null ? 0 : grid1[i, j];

                    linesY[j] = grid[j, i];
                    linesY1[j] = grid1 == null ? 0 : grid1[j, i];
                }

                if (CommonOperations != null)
                {
                    CommonOperations(this, new SudokuPacketEventArgs
                    {
                        PacketCells = cells,
                        PacketLinesX = linesX,
                        PacketLinesY = linesY,
                        PacketCells1 = cells1,
                        PacketLinesX1 = linesX1,
                        PacketLinesY1 = linesY1,
                        SmallGrid = smallGrid,
                        SmallGrid1 = smallGrid1,
                        X = ni,
                        Y = nj
                    });
                }

                if ((i + 1) % N == 0)
                {
                    nj += N;
                }
                ni += N;
            }
        }

        public class SudokuPacketEventArgs : EventArgs
        {
            public int[] PacketCells { get; set; }

            public int[] PacketLinesX { get; set; }

            public int[] PacketLinesY { get; set; }

            public int[] PacketCells1 { get; set; }

            public int[] PacketLinesX1 { get; set; }

            public int[] PacketLinesY1 { get; set; }

            public int[,] SmallGrid { get; set; }

            public int[,] SmallGrid1 { get; set; }

            public int X { get; set; }

            public int Y { get; set; }
        }
    }
}
