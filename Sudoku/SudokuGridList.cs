using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuGridList
    {
        private List<SudokuGrid> _sudokuGrids;

        public List<SudokuGrid> SudokuGrids
        {
            get { return _sudokuGrids; }
            set { _sudokuGrids = value; }
        }

    }
}
