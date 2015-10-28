using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Subject
    {
        public static double ScoreTotal;

        private SudokuGrid _sudokuGrid;

        public SudokuGrid SudokuGrid
        {
            get
            {
                return _sudokuGrid;
            }
            set
            {
                _sudokuGrid = value;
            }
        }

        private Rate _rate;

        public Rate Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
            }
        }

        public static Subject CreateSudokuSubject(int n)
        {
            return new Subject(new SudokuGrid(n, null, false));
        }

        public Subject(SudokuGrid sudokuGrid)
        {
            _sudokuGrid = sudokuGrid;
            _rate = new Rate();
        }

        public Subject(int n, bool?[,][] gridTrinaire)
        {
            _sudokuGrid = new SudokuGrid(n, gridTrinaire);
        }
    }
}