using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Rate
    {
        private double _score;
        private double _packetCellsScore;
        private double _packetLineXScore;
        private double _packetLineYScore;
        private double _rank;

        public Rate(double score, double? packetCellsScore = null, double? packetLineXScore = null, double? packetLineYScore = null)
        {
            _score = score;
            _packetCellsScore = packetCellsScore ?? new double();
            _packetLineXScore = packetLineXScore ?? new double();
            _packetLineYScore = packetLineYScore ?? new double();
        }

        public Rate()
        {
        }

        public double Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public double Chance { get; set; }

        public double Score2
        {
            get { return _score * _score; }
        }

        public double ChanceInf { get; set; }

        public double ChanceSup
        {
            get { return ChanceInf + Chance; }
        }
        public double ChanceSup2
        {
            get { return ChanceInf2 + Chance2; }
        }
        public double Chance2 { get; set; }
        public double ChanceInf2 { get; set; }

        public double PacketCellsScore
        {
            get { return _packetCellsScore; }
            set { _packetCellsScore = value; }
        }

        public double PacketLineXScore
        {
            get { return _packetLineXScore; }
            set { _packetLineXScore = value; }
        }

        public double PacketLineYScore
        {
            get { return _packetLineYScore; }
            set { _packetLineYScore = value; }
        }

        public double Rank
        {
            get { return _rank; }
            set { _rank = value; }
        }

        public double ChanceRank { get; set; }

        public double ChanceRankInf { get; set; }

        public double ChanceRankSup
        {
            get { return ChanceRankInf + ChanceRank; }
        }
    }
}
