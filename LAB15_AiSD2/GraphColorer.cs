using System;
using System.Collections.Generic;
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
            int numberOfColors = Int32.MaxValue;
            int[] coloring = new int[g.VertexCount];
            for (int i = 0; i < coloring.Length; i++)
                coloring[i] = -1;
            coloring[0] = 0;
            bool[] visited = new bool[g.VertexCount];
            for (int i = 0; i < g.VertexCount; i++)
            {
                if (!visited[i])
                {
                    var temp = FindBestColoringRec(g, 1, i, coloring, visited);
                    if (temp.numberOfColors > numberOfColors)
                    {
                        numberOfColors = temp.numberOfColors;
                        coloring = temp.coloring;
                    }

                    visited = temp.visited;
                }
            }

            return (numberOfColors, coloring);
        }


        public (int numberOfColors, int[] coloring, bool[] visited) FindBestColoringRec(Graph g, int numberOfColors, 
            int v, int[] coloring, bool[] visited)
        {
            // Save current vertex (to not return to it)
            visited[v] = true;
            
            // If degree of vertex is smaller than current nr. of colors I should leave this vertex to color at the end
            if (g.Degree(v) < numberOfColors)
            {
                // moge pokolorowac na kazdy z kolorow (jesli zakladamy ze chodzimy tylko po nowych, wtedy trzbea dodac
                // warunek konczacy prace jesli jestes na koncu)
            }
            
            // All is OK


        }
        
    }
}