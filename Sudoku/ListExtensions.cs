using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public static class ListExtensions
    {
        public static List<T[]> GroupRandomlyByPairs<T>(this List<T> list, Random random)
        {
            List<T[]> pairs = new List<T[]>();
            int iterationMax = list.Count / 2;
            for (int i = 0; i < iterationMax; i++)
            {
                int k = random.Next(iterationMax - i);
                T item1 = list.ElementAt(k);
                list.RemoveAt(k);

                int l = random.Next(iterationMax - i - 1);
                T[] pair = { item1, list.ElementAt(l) };
                list.RemoveAt(l);

                pairs.Add(pair);
            }
            return pairs;
        }

    }
}
