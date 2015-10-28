using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuGrid
    {
        private int _n;
        private int _n2;

        public int N
        {
            get { return _n; }
            set { _n = value; }
        }

        public int N2
        {
            get { return _n2; }
            set { _n2 = value; }
        }

        private int[,] _grid;

        private bool?[,][] _gridTrinaire;

        public int[,] Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        public bool?[,][] GridTrinaire
        {
            get { return _gridTrinaire; }
            set { _gridTrinaire = value; }
        }

        public SudokuGrid(int n, int[,] grid = null, bool generate = false)
        {
            _n = n;
            _n2 = n * n;
            _grid = grid ?? (generate ? GenerateRandomTable(n) : new int[_n2, _n2]);
            _gridTrinaire = new bool?[_n2, _n2][];

            if (grid != null || generate)
            {
                for (int i = 0; i < _n2; i++)
                {
                    for (int j = 0; j < _n2; j++)
                    {
                        _gridTrinaire[i, j] = IntToTrinaire(_grid[i, j]);
                    }
                }
            }
        }

        public SudokuGrid(int n, bool?[,][] gridTrinaire)
        {
            _n = n;
            _n2 = n * n;
            _grid = new int[_n2, _n2];
            _gridTrinaire = gridTrinaire;

            if (gridTrinaire != null && gridTrinaire.LongLength == _n2 * _n2)
            {
                for (int i = 0; i < _n2; i++)
                {
                    for (int j = 0; j < _n2; j++)
                    {
                        _grid[i, j] = TrinaireToInt(gridTrinaire[i, j]);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Dimension du tableau invalide");
            }
        }

        private int[,] GenerateRandomTable(int n)
        {
            int n2 = n * n;
            int[,] grid = new int[n2, n2];
            Random rand = new Random();
            for (int i = 0; i < n2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    grid[i, j] = rand.Next(n2);
                }
            }
            return grid;
        }

        public static List<SudokuGrid> GetRandomGrids(int n, int gridCount, Random random)
        {
            List<SudokuGrid> grids = new List<SudokuGrid>();

            for (int k = 0; k < gridCount; k++)
            {
                int n2 = n * n;
                int[,] grid = new int[n2, n2];
                for (int i = 0; i < n2; i++)
                {
                    for (int j = 0; j < n2; j++)
                    {

                        grid[i, j] = random.Next(n2);
                    }
                }
                grids.Add(new SudokuGrid(n, grid));
            }
            return grids;
        }

        public static List<SudokuGrid> GetRandomGridsWithBase(Random rand, int n, int gridCount, int?[,] gridBase)
        {
            List<SudokuGrid> grids = new List<SudokuGrid>();

            for (int k = 0; k < gridCount; k++)
            {
                int n2 = n * n;
                int[,] grid = new int[n2, n2];
                for (int i = 0; i < n2; i++)
                {
                    for (int j = 0; j < n2; j++)
                    {
                        if (gridBase[i, j] != null)
                        {
                            grid[i, j] = gridBase[i, j].Value - 1;
                        }
                        else
                        {
                            grid[i, j] = rand.Next(n2);
                        }
                    }
                }
                grids.Add(new SudokuGrid(n, grid));
            }
            return grids;
        }

        bool?[] IntToTrinaire(int i)
        {
            if (i < 0 || i > _n2 - 1)
            {
                throw new InvalidOperationException("La valeur à traduite est en dehors des limites");
            }
            switch (i)
            {
                case 0:
                    return new bool?[] { null, null };
                case 1:
                    return new bool?[] { null, false };
                case 2:
                    return new bool?[] { null, true };
                case 3:
                    return new bool?[] { false, null };
                case 4:
                    return new bool?[] { false, false };
                case 5:
                    return new bool?[] { true, false };
                case 6:
                    return new bool?[] { true, null };
                case 7:
                    return new bool?[] { true, false };
                case 8:
                    return new bool?[] { true, true };
                default:
                    throw new InvalidOperationException("La valeur à traduite est en dehors des limites");
            }
        }

        int TrinaireToInt(bool?[] t)
        {
            if (t.Count() != 2)
            {
                throw new InvalidOperationException("La valeur à traduire est en dehors des limites");
            }
            if (t[0].Equals(null) && t[1].Equals(null))
            {
                return 0;
            }
            if (t[0].Equals(null) && t[1].Equals(false))
            {
                return 1;
            }
            if (t[0].Equals(null) && t[1].Equals(true))
            {
                return 2;
            }
            if (t[0].Equals(false) && t[1].Equals(null))
            {
                return 3;
            }
            if (t[0].Equals(false) && t[1].Equals(false))
            {
                return 4;
            }
            if (t[0].Equals(false) && t[1].Equals(true))
            {
                return 5;
            }
            if (t[0].Equals(true) && t[1].Equals(null))
            {
                return 6;
            }
            if (t[0].Equals(true) && t[1].Equals(false))
            {
                return 7;
            }
            if (t[0].Equals(true) && t[1].Equals(true))
            {
                return 8;
            }
            throw new Exception("Impossible de traduire la valeur");
        }
    }
}
