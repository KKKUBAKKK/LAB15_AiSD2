using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ASD;
using ASD.Graphs;

namespace ASD2
{
    public class GraphColorer : MarshalByRefObject
    {
        /// <summary>
        /// Metoda znajduje kolorowanie zadanego grafu g używające najmniejsze możliwej liczby kolorów.
        /// </summary>
        /// <param name="g">Graf (nieskierowany)</param>
        /// <returns>Liczba użytych kolorów i kolorowanie (coloring[i] to kolor wierzchołka i). Kolory mogą być
        /// dowolnymi liczbami całkowitymi.</returns>
        public (int numberOfColors, int[] coloring) FindBestColoring(Graph g)
        {
            return (0, new int[0]);
        }
        
        // Wybor wierzcholka, ktory ma mniej dostepnych kolorow niz niepokolorowanych sasiadow i ma z takich
        // wierzcholkow najmniej dostepnych kolorow, a jesli tyle samo, to najwiecej niepokolorowanych sasiadow uncolored, coloring, avcolors, avneighbors
        public int FindVertexToColor(List<int> coloring, List<int> freeVertices, List<int> freeColors, List<int> freeNeighbors,
            System.Collections.Generic.Stack<int> cutoffStack)//, ref int cutoffNumber)//, List<(int, int)> cutoffList)
        {
            int res = -1;
            for (int i = 0; i < freeVertices.Count; i++)
            {
                int v = freeVertices[i];
                if (freeNeighbors[i] < freeColors[i])
                {
                    freeVertices.Remove(v);
                    i--;
                    coloring[v] = -1;
                    // cutoffList.Add((v, freeColors[v]));
                    // cutoffNumber++;
                    cutoffStack.Push(v);
                }
                else if (res == -1 || freeColors[res] > freeColors[v] || 
                         (freeColors[res] == freeColors[v] && freeNeighbors[v] > freeNeighbors[res]))
                {
                    res = v;
                }
            }

            return res;
        }
        
        // Funkcja obsluguje pokolorowanie wszystkich wierzcholkow
        public void CheckColoring(ref int numberOfColors, ref int bestNumberOfColors, 
            // List<(int, int)> cutoffList, List<int> bestCutoffList, 
            bool[,] takenColors, bool[,] bestTakenColors,
            List<int> coloring, List<int> bestColoring,
            List<int> freeVertices, 
            List<int> freeColors, List<int> bestFreeColors,
            List<int> freeNeighbors,
            System.Collections.Generic.Stack<int> cutoffStack, List<int> bestCutoffList)//,
            // ref int cutoffNumber)
        {

            if (numberOfColors < bestNumberOfColors)
            {
                bestNumberOfColors = numberOfColors;
                bestTakenColors = (bool[,])takenColors.Clone();
                bestColoring.Clear();
                bestColoring.AddRange(coloring);
                bestFreeColors.Clear();
                bestFreeColors.AddRange(freeColors);
            }
            
            while (cutoffStack.Count > 0)
                freeVertices.Add(cutoffStack.Pop());
        }
        
        // Funkcja wywoluje rekurencje na kazdym mozliwym kolorze, przed przygotowuje dane, a po przywraca je
        public void RecursiveCall(int v, int numberOfColors, bool[,] takenColors, List<int> freeVertices, 
            List<int> freeColors, List<int> freeNeighbors, List<int> coloring, Graph g)
        {
            var log = new System.Collections.Generic.Stack<(int, bool)>();
            freeVertices.Remove(v);
            
            for (int i = 0; i < numberOfColors; i++)
            {
                if (takenColors[v, i])
                    continue;

                coloring[v] = i;

                foreach (var neighbor in g.OutNeighbors(v))
                {
                    if (!takenColors[neighbor, i])
                    {
                        log.Push((neighbor, true));
                        takenColors[neighbor, i] = true;
                        freeColors[neighbor]--;
                    }
                    else
                        log.Push((neighbor, false));

                    freeNeighbors[neighbor]--;
                }
                
                // TODO: WYWOLANIE REKURENCYJNE GLOWNEJ FUNKCJI TUTAJ

                while (log.Count > 0)
                {
                    var transaction = log.Pop();
                    if (transaction.Item2)
                    {
                        takenColors[transaction.Item1, i] = false;
                        freeColors[transaction.Item1]++;
                    }

                    freeNeighbors[transaction.Item1]++;
                }
                
                log.Clear();
            }
            
            coloring[v] = -2;
            freeVertices.Add(v);
        }
    }
}