using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Sudoku;

namespace Sudoku
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Population Count - Mutation Coef - Renewal Count - Dominance Coef");
            if (args.Length == 0)
            {
                var readLine = Console.ReadLine();
                if (readLine != null) args = readLine.Split(' ');
                else Main(args);
            }

            /* Sans SetDominant()*/
            /* 1000 300 0 => 237*/
            /* 1000 250 0 => 239*/
            /* 1000 225 generation : 93 score : 238 */
            /* 1000 200 generation : 100 score : 239 */
            /* 1250 250 95 241*/
            /* 1500 250 106 237*/

            /* Avec SetDominant */
            /* 1000 250 0 => 241*/
            /* 1000 250 0 => 233*/
            /* 1000 250 10 => 241 SetDominant à 70*/


            int n = 3; // 3
            int count = int.Parse(args[0]); // 100
            int mutationCoef = int.Parse(args[1]); // 1000
            int renewalCount = int.Parse(args[2]); // 5
            double dominanceRatio = double.Parse(args[3]) / 100; // 100

            string consoleTitle = " ind : {0} muta° : {1} renewal : {2} generation : {3} dom : {4} diversité : {5}";

            int n2 = n * n;

            int?[,] sudokuBase = 
            {
                {3,7,5,null,null,null,2,null,null},
                {null,null,8,null,null,3,null,null,7},
                {null,1,null,null,null,9,null,null,null},
                {null,6,null,3,2,null,null,null,5},
                {null,null,null,1,null,8,null,null,null},
                {null,null,null,4,null,null,7,2,null},
                {null,4,2,null,null,null,null,3,null},
                {5,3,null,8,null,null,null,null,4},
                {8,null,null,null,null,null,null,null,null}
            };

            //int?[,] sudokuBase =
            //{
            //    {null, null, 8, 9, null, null, null, null, null},
            //    {null, null, null, 4, 2, null, null, null, 6},
            //    {6, null, null, 5, null, null, null, 9, 3},
            //    {3, null, 4, null, null, null, 9, 8, 5},
            //    {null, null, 2, null, null, null, null, 6, null},
            //    {1, null, null, null, 3, null, null, null, null},
            //    {9, null, null, null, null, null, null, 5, 2},
            //    {null, null, 3, null, null, null, null, null, null},
            //    {2, 4, 1, null, 5, null, null, null, null}
            //};

            int?[,] sudokuBaseFixed = sudokuBase;

            Random random = new Random();
            Population population = new Population(n, count, random, true, sudokuBase);
            int generation = new int();
            int score = new int();
            GridOperations gridOperation = new GridOperations(n);

            gridOperation.CommonOperations += delegate(object sender, GridOperations.SudokuPacketEventArgs eventArgs)
            {
                score += Population.GetScore(eventArgs.PacketCells);
                score += Population.GetScore(eventArgs.PacketLinesX);
                score += Population.GetScore(eventArgs.PacketLinesY);
            };

            while (true)
            {
                int diversityRate = population.GetDiversityRate();
                Console.Title = string.Format(consoleTitle, count, mutationCoef, renewalCount, generation, dominanceRatio, diversityRate);

                // Calcul des notes de chacun des sudoku
                population.CalculateRates();
                // Si un numéro est dominant à XX % alors on le rend persistant
                sudokuBase = population.SetDominant(sudokuBase, dominanceRatio);
                // On selectionne la population à reproduire
                Population populationSelected = population.GetRandomPopulation(random);
                // On groupe la population sélectionnée par pair de 2
                List<SubjectPair> subjectPairs = populationSelected.GroupRandomlyByPairs(random);

                // Pour chaque pair on fabrique les descendants
                List<Subject> siblings = new List<Subject>();
                foreach (var subjectPair in subjectPairs)
                {
                    var siblingPairs = subjectPair.GetSiblings(random, sudokuBase, mutationCoef);
                    siblingPairs.AddRange(new[] { subjectPair.SubjectFemale, subjectPair.SubjectMale });

                    // On prend les meilleurs des parents et descendants
                    foreach (var siblingPair in siblingPairs)
                    {
                        gridOperation.GetGridScore(siblingPair.SudokuGrid.Grid);
                        siblingPair.Rate.Score = score;
                        score = 0;
                    }
                    siblings.AddRange(siblingPairs.OrderByDescending(s => s.Rate.Score).Take(2));
                }

                // On renouvelle la population pour éviter la convergence
                Population populationRenewal = new Population(n, renewalCount, random, true, sudokuBase);
                for (int i = 0; i < renewalCount; i++)
                {
                    int renew = random.Next(count);
                    siblings[renew] = populationRenewal.Subjects[i];
                }

                // On calculs les notes de la nouvelle génération
                Population populationSiblings = new Population(n, count, random, false, null, siblings);
                populationSiblings.CalculateRates();

                // Si la nouvelle génération est meilleure que la précédente on incrémente
                if (populationSiblings.ScoreTotal > population.ScoreTotal)
                {
                    population = populationSiblings;
                    Console.WriteLine(" Maximums : {0} ", population.Subjects.Count(s => s.Rate.Score == 243d));
                    if (population.Subjects.Any(s => s.Rate.Score == 243d))
                    {
                        Console.ReadLine();
                    }
                    Console.WriteLine(" Génération n + 1 ");
                }

                // Affichage du score minimum et maximum
                double scoreMin = population.Subjects.Min(s => s.Rate.Score);
                double scoreMax = population.Subjects.Max(s => s.Rate.Score);
                Console.WriteLine(" Score maximum : {0} \n Score minimum : {1} \n", scoreMax, scoreMin);

                // Si le score du meilleur est égal au score du pire alors on stoppe l'évolution
                if (scoreMin == scoreMax)
                {
                    DisplaySudokuGrid(population.Subjects.First().SudokuGrid.Grid, sudokuBaseFixed, n, n2);
                    Console.ReadLine();
                    break;
                }
                if (NativeKeyboard.IsKeyDown(KeyCode.Left))
                {
                    int subjectCount = new int();
                    foreach (var subject in population.Subjects)
                    {
                        ColoredConsoleWrite(ConsoleColor.Green, string.Format("   ###### {0} # Score : {1}", subjectCount, subject.Rate.Score), true);
                        DisplaySudokuGrid(subject.SudokuGrid.Grid, sudokuBaseFixed, n, n2);
                        if (Console.ReadKey().Key != ConsoleKey.LeftArrow)
                        {
                            break;
                        }
                        subjectCount++;
                    }
                }
                generation++;
            }
        }

        private static void DisplaySudokuGrid(int[,] grid, int?[,] gridBase, int n, int n2)
        {
            Console.WriteLine();
            for (int i = 0; i < n2; i++)
            {
                if (i != 0 && i % n == 0)
                { Console.WriteLine("       ++       ++"); }
                else
                { Console.WriteLine(); }
                for (int j = 0; j < n2; j++)
                {
                    if (gridBase[i, j] != null)
                    {
                        ColoredConsoleWrite(ConsoleColor.Red, string.Format("{0}  ", grid[i, j] + 1));
                    }
                    else
                    {
                        Console.Write("{0}  ", grid[i, j] + 1);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text, bool writeLine = false)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
            if (writeLine)
            {
                Console.Write("\n");
            }
        }


        internal enum KeyCode : int
        {
            /// <summary>
            /// The left arrow key.
            /// </summary>
            Left = 0x25,

            /// <summary>
            /// The up arrow key.
            /// </summary>
            Up,

            /// <summary>
            /// The right arrow key.
            /// </summary>
            Right,

            /// <summary>
            /// The down arrow key.
            /// </summary>
            Down
        }

        /// <summary>
        /// Provides keyboard access.
        /// </summary>
        internal static class NativeKeyboard
        {
            /// <summary>
            /// A positional bit flag indicating the part of a key state denoting
            /// key pressed.
            /// </summary>
            private const int KeyPressed = 0x8000;

            /// <summary>
            /// Returns a value indicating if a given key is pressed.
            /// </summary>
            /// <param name="key">The key to check.</param>
            /// <returns>
            /// <c>true</c> if the key is pressed, otherwise <c>false</c>.
            /// </returns>
            public static bool IsKeyDown(KeyCode key)
            {
                return (GetKeyState((int)key) & KeyPressed) != 0;
            }

            /// <summary>
            /// Gets the key state of a key.
            /// </summary>
            /// <param name="key">Virtuak-key code for key.</param>
            /// <returns>The state of the key.</returns>
            [DllImport("user32.dll")]
            private static extern short GetKeyState(int key);
        }
    }
}