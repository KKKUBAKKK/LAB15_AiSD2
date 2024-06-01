using System;
using System.Collections.Generic;
using System.Drawing;
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
            int[] coloring = new int[g.VertexCount];
            Array.Fill(coloring, -1);

            int numberOfColors;
            for (numberOfColors = 1; numberOfColors <= g.VertexCount; numberOfColors++)
            {
                if (GreedyColoring(g, 0, coloring, numberOfColors))
                    break;
            }

            return (numberOfColors, coloring);
        }

        public bool[] AvailableColors(Graph g, int v, int[] coloring, int numberOfColors)
        {
            bool[] used = new bool[numberOfColors];

            foreach (var n in g.OutNeighbors(v))
            {
                if (coloring[n] != -1)
                    used[coloring[n]] = true;
            }

            return used;
        }

        public bool GreedyColoring(Graph g, int v, int[] coloring, int numberOfColors)
        {
            if (v == g.VertexCount)
                return true;
            
            var used = AvailableColors(g, v, coloring, numberOfColors);

            for (int i = 0; i < used.Length; i++)
            {
                if (used[i])
                    continue;

                coloring[v] = i;
                if (GreedyColoring(g, v + 1, coloring, numberOfColors))
                    return true;
                coloring[v] = -1;
            }

            return false;
        }
        
    }
}