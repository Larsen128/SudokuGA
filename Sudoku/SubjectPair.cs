using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SubjectPair
    {
        private Subject _subjectFemale;

        private Subject _subjectMale;

        private GridOperations _gridOperations;

        public Subject SubjectFemale
        {
            get
            {
                return _subjectFemale;
            }
            set
            {
                _subjectFemale = value;

            }
        }

        public Subject SubjectMale
        {
            get
            {
                return _subjectMale;

            }
            set
            {
                _subjectMale = value;
            }
        }

        public SubjectPair(Subject subjectMale, Subject subjectFemale)
        {
            _subjectMale = subjectMale;
            _subjectFemale = subjectFemale;
            _gridOperations = new GridOperations(_subjectFemale.SudokuGrid.N);
        }

        public List<Subject> GetSiblings(Random random, int?[,] sudokuBase, int mutationCoef)
        {
            int n = _subjectMale.SudokuGrid.N;
            int n2 = _subjectMale.SudokuGrid.N2;
            Subject siblingFemale = Subject.CreateSudokuSubject(n);
            Subject siblingMale = Subject.CreateSudokuSubject(n);

            _gridOperations.CommonOperations += delegate(object sender, GridOperations.SudokuPacketEventArgs args)
            {
                int nbCells = random.Next(1, n2);
                List<int> cellPossible = new List<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
                List<int> cells = new List<int>();

                for (int k = 0; k < nbCells; k++)
                {
                    int cell = random.Next(cellPossible.Count);
                    cells.Add(cellPossible.ElementAt(cell));
                    cellPossible.RemoveAt(cell);
                }

                #region
                /* Numérotation des cases :
                 1 2 3      1 : [0,0] 2 : [0,1] 3 : [0,2]
                 4 5 6      4 : [1,0] 5 : [1,1] 6 : [1,2]
                 7 8 9      7 : [2,0] 8 : [2,1] 9 : [2,2]
                 */
                #endregion

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        // i,j => k
                        int k = j + (3 * i);
                        if (cells.Contains(k))
                        {
                            siblingFemale.SudokuGrid.Grid[args.X + i, args.Y + j] = args.SmallGrid[i, j];
                            siblingMale.SudokuGrid.Grid[args.X + i, args.Y + j] = args.SmallGrid1[i, j];
                        }
                        else
                        {
                            siblingFemale.SudokuGrid.Grid[args.X + i, args.Y + j] = args.SmallGrid1[i, j];
                            siblingMale.SudokuGrid.Grid[args.X + i, args.Y + j] = args.SmallGrid[i, j];
                        }
                    }
                }
                // Mutation
                if (random.Next(0, mutationCoef) == 1) // 1/100
                {
                    Mutate(random, sudokuBase, n2, n, args, siblingFemale);
                }
                if (random.Next(0, mutationCoef) == 0) // 1/100
                {
                    Mutate(random, sudokuBase, n2, n, args, siblingMale);
                }
            };

            _gridOperations.GetGridScore(_subjectFemale.SudokuGrid.Grid, _subjectMale.SudokuGrid.Grid);
            return new List<Subject>(new[] { siblingMale, siblingFemale });
        }

        private static void Mutate(Random random, int?[,] sudokuBase, int n2, int n, GridOperations.SudokuPacketEventArgs args, Subject siblingFemale)
        {
            int cellMutated1 = random.Next(0, n2);
            int ii1 = cellMutated1 % n;
            int jj1 = (cellMutated1 - ii1) / n;

            int cellMutated2 = random.Next(0, n2 - 1);
            List<int> cellsList = new List<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
            cellsList.RemoveAt(cellMutated1);
            cellMutated2 = cellsList.ElementAt(cellMutated2);
            int ii2 = cellMutated2 % 2;
            int jj2 = (cellMutated2 - ii2) / n;

            if (sudokuBase[args.X + ii1, args.Y + jj1] == null && sudokuBase[args.X + ii2, args.Y + jj2] == null)
            {
                int temp = siblingFemale.SudokuGrid.Grid[args.X + ii1, args.Y + jj1];
                siblingFemale.SudokuGrid.Grid[args.X + ii1, args.Y + jj1] = siblingFemale.SudokuGrid.Grid[args.X + ii2, args.Y + jj2];
                siblingFemale.SudokuGrid.Grid[args.X + ii2, args.Y + jj2] = temp;
                Console.WriteLine("\n+++++ Mutation +++++\n");
            }
        }
    }
}

