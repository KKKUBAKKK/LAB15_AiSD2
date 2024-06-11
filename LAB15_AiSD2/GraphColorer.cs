using System;
using System.Collections.Generic;
using System.Linq;
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
            for (int i = 0; i < g.VertexCount; i++)
                coloring[i] = -1;

            var res = FindBestColoringRec(g, 0, coloring, 0);
            return res;
        }

        // Funkcja dziala w ten sposob ze jesli nie znajdzie 0 <= kolor < numberOfColors, to zwraca nowy
        public (int color, int n, int f, bool[] used) FindSmallestAvailableColor(Graph g, int v, int[] coloring, int numberOfColors)
        {
            bool[] used = new bool[numberOfColors];
            int n = 0;
            foreach (var neighbor in g.OutNeighbors(v))
            {
                if (coloring[neighbor] != -1)
                    used[coloring[neighbor]] = true;
                else
                    n++;
            }

            int f = used.Select((b => (b) ? 0 : 1)).Sum();
            
            for (int i = 0; i < used.Length; i++)
                if (!used[i])
                    return (i, n, f, used);

            return (numberOfColors, n, f, used);
        }

        public (int numberOfColors, int[] coloring) FindBestColoringRec(Graph g, int v, int[] coloring, 
            int numberOfColors, int maxNumberOfColors = Int32.MaxValue)
        {
            // If there is a coloring with maxNumberOfColors < than current numberOfColors, stop this branch
            if (numberOfColors >= maxNumberOfColors)
                return (Int32.MaxValue, new int[0]);
            
            // If we are at the last vertex, then the coloring is complete
            if (v >= g.VertexCount)
            {
                var res = (int[])coloring.Clone();
                return (numberOfColors, res);
            }
            
            (int color, int n, int f, bool[] used) c = FindSmallestAvailableColor(g, v, coloring, numberOfColors);
            if (c.color >= maxNumberOfColors)
                return (Int32.MaxValue, new int[0]);
            
            // // If we are at the last vertex, then the coloring is complete
            // if (v >= g.VertexCount)
            // {
            //     var res = (int[])coloring.Clone();
            //     res[v] = c.color;
            //     if (c.color >= numberOfColors)
            //         numberOfColors++;
            //     return (numberOfColors, res);
            // }
            
            // moze powinienem ustawiac -1 dla usunietych (na chwile) wierzcholkow a 0 dla niuepokolorowanhych???
            if (c.n < c.f)
            {
                (int numberOfColors, int[] coloring) temp = FindBestColoringRec(g, v + 1, coloring, numberOfColors, maxNumberOfColors);
                if (temp.numberOfColors >= maxNumberOfColors)
                    return (Int32.MaxValue, new int[0]);
                (int color, int n, int f, bool[] used) t =
                    FindSmallestAvailableColor(g, v, temp.coloring, temp.numberOfColors);
                var res = (int[])temp.coloring.Clone();
                res[v] = t.color;
                
                return (temp.numberOfColors, res);
            }
            
            (int numberOfColors, int[] coloring) best = (int.MaxValue, new int[0]);
            int tf = 0;
            if (c.color == numberOfColors)
                numberOfColors++;
            for (int i = c.color; i < numberOfColors; i++)
            {
                if (tf > c.f)
                    break;
                
                if (i < c.used.Length && c.used[i])
                    continue;
                
                tf++;
                coloring[v] = i;
                (int numberOfColors, int[] coloring) temp = FindBestColoringRec(g, v + 1, coloring, numberOfColors, Math.Min(best.numberOfColors, maxNumberOfColors));
                if (temp.numberOfColors < best.numberOfColors)
                {
                    best.numberOfColors = temp.numberOfColors;
                    best.coloring = (int[])temp.coloring.Clone();
                }
                coloring[v] = -1;
            }
            
            return best;
        }
    }
}