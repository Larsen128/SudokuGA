using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Population
    {
        private int _count;

        private int _n;

        public int _n2;

        public int Count { get; set; }

        private List<Subject> _subjects;

        public List<Subject> Subjects
        {
            get { return _subjects; }
            set { _subjects = value; }
        }

        private double _score;

        public Population(int n, int count, Random rand, bool random = true, int?[,] gridBase = null, List<Subject> subjects = null)
        {
            _n = n;
            _n2 = n * n;
            _count = count;
            if (random && gridBase == null)
            {
                _subjects = new List<Subject>(SudokuGrid.GetRandomGrids(n, count, rand).Select(s => new Subject(s)));
            }
            else if (random)
            {
                _subjects = new List<Subject>(SudokuGrid.GetRandomGridsWithBase(rand, n, count, gridBase).Select(s => new Subject(s)));
            }
            else if (subjects != null)
            {
                _subjects = new List<Subject>(subjects.Select(s => new Subject(s.SudokuGrid)));
            }

            _operations = new GridOperations(n);

            _operations.CommonOperations += delegate(object sender, GridOperations.SudokuPacketEventArgs eventArgs)
            {
                _score += GetScore(eventArgs.PacketCells);
                _score += GetScore(eventArgs.PacketLinesX);
                _score += GetScore(eventArgs.PacketLinesY);
            };
        }

        public void CalculateRates()
        {
            int scorePacketCells = new int();
            int scorePacketLineX = new int();
            int scorePacketLineY = new int();

            GridOperations operations = new GridOperations(_n);
            operations.CommonOperations += delegate(object sender, GridOperations.SudokuPacketEventArgs eventArgs)
            {
                scorePacketCells += GetScore(eventArgs.PacketCells);
                scorePacketLineX += GetScore(eventArgs.PacketLinesX);
                scorePacketLineY += GetScore(eventArgs.PacketLinesY);
            };
            foreach (var subject in _subjects)
            {
                operations.GetGridScore(subject.SudokuGrid.Grid);
                subject.Rate = new Rate(scorePacketCells + scorePacketLineX + scorePacketLineY, scorePacketCells, scorePacketLineX, scorePacketLineY);
                scorePacketCells = 0;
                scorePacketLineX = 0;
                scorePacketLineY = 0;
            }
            _scoreTotal = GetScoreTotal();
            _score2Total = GetScore2Total();

            double chanceSum = new double();
            double chanceSum2 = new double();
            foreach (var subject in _subjects)
            {
                subject.Rate.Chance = subject.Rate.Score / _scoreTotal;
                subject.Rate.Chance2 = subject.Rate.Score2 / _score2Total;
                subject.Rate.ChanceInf = chanceSum;
                subject.Rate.ChanceInf2 = chanceSum2;
                chanceSum += subject.Rate.Chance;
                chanceSum2 += subject.Rate.Chance2;

                subject.Rate.Rank = _subjects.Count(s => s.Rate.PacketCellsScore > subject.Rate.PacketCellsScore && s.Rate.PacketLineXScore > subject.Rate.PacketLineXScore && s.Rate.PacketLineYScore > subject.Rate.PacketLineYScore) + 1;
            }

            chanceSum = 0;
            double scoreTotal = _subjects.Sum(s => 1 / s.Rate.Rank);
            foreach (var subject in _subjects)
            {
                subject.Rate.ChanceRank = subject.Rate.Rank == 0 ? 0 : (1 / subject.Rate.Rank) / scoreTotal;
                subject.Rate.ChanceRankInf = chanceSum;
                chanceSum += subject.Rate.ChanceRank;
            }

            Console.WriteLine(" Score Total = {0} ", _scoreTotal);
        }

        public int?[,] SetDominant(int?[,] gridBase, double dominanceRatio)
        {
            int?[,] grid = new int?[_n2, _n2];
            for (int i = 0; i < _n2; i++)
            {
                for (int j = 0; j < _n2; j++)
                {
                    if (_subjects.GroupBy(s => s.SudokuGrid.Grid[i, j]).Select(s => s.Count()).Any(s => s > _count * dominanceRatio))
                    {
                        grid[i, j] = _subjects.GroupBy(s => s.SudokuGrid.Grid[i, j]).SingleOrDefault(s => s.Count() > _count * dominanceRatio).Key + 1;
                    }
                }
            }
            return grid;
            return "";
        }
         
        public int GetDiversityRate()
        {
            int diversityRate = new int();
            for (int i = 0; i < _n2; i++)
            {
                for (int j = 0; j < _n2; j++)
                {
                    diversityRate += _subjects.GroupBy(s => s.SudokuGrid.Grid[i, j]).Count();
                }
            }
            return diversityRate;
        }

        public Population GetRandomPopulation(Random random)
        {
            List<Subject> subjects = new List<Subject>();
            for (int i = 0; i < _count; i++)
            {
                double randomDouble = random.NextDouble();
                subjects.Add(_subjects.SingleOrDefault(s => randomDouble >= s.Rate.ChanceInf && randomDouble < s.Rate.ChanceSup));
            }
            return new Population(_n, _count, null, false, null, subjects);
        }

        public Population GetRandomPopulationByRank(Random random)
        {
            List<Subject> subjects = new List<Subject>();
            for (int i = 0; i < _count; i++)
            {
                double randomDouble = random.NextDouble();
                subjects.Add(_subjects.SingleOrDefault(s => randomDouble >= s.Rate.ChanceRankInf && randomDouble < s.Rate.ChanceRankSup));
            }
            return new Population(_n, _count, null, false, null, subjects);
        }

        private double GetScoreTotal()
        {
            _scoreTotal = _subjects.Sum(s => s.Rate.Score);
            return _scoreTotal;
        }

        private double GetScore2Total()
        {
            _score2Total = _subjects.Sum(s => s.Rate.Score2);
            return _score2Total;
        }

        private double _scoreTotal;

        public double ScoreTotal { get { return _scoreTotal; } }

        private double _score2Total;

        private GridOperations _operations;

        public GridOperations Operations
        {
            get
            {
                return _operations;
            }
            set
            {
                _operations = value;

            }
        }

        public static int GetScore(int[] cells)
        {
            int score = cells.GroupBy(c => c).Count();
            return score;
        }

        public List<SubjectPair> GroupRandomlyByPairs(Random random)
        {
            return Subjects.GroupRandomlyByPairs(random).Select(x => new SubjectPair(x[0], x[1])).ToList();
        }
    }
}
