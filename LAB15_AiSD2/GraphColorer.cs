using System;
using System.Collections.Generic;
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
            return Brown(g);
        }

        public (int numberOfColors, int[] coloring) Brown(Graph g)
        {
            int n = g.VertexCount;
            var vertices = Enumerable.Range(0, n).OrderByDescending(v => g.Degree(v)).ToArray();
            int[] coloring = new int[n];
            coloring[vertices[0]] = 1;
            int i = 2, k = n, q = 1;
            List<int>[] U = new List<int>[n];
            for (int j = 0; j < n; j++)
                U[j] = new List<int>();
            int[] l = new int[n];
            l[0] = 1;
            bool updateU = true;

            while (i > 1)
            {
                if (updateU)
                {
                    int vi = vertices[i - 1];
                    U[i - 1] = new List<int>(Enumerable.Range(1, q + 1));
                    foreach (var neighbor in g.OutNeighbors(vi))
                    {
                        if (coloring[neighbor] != 0)
                            U[i - 1].Remove(coloring[neighbor]);
                    }
                }

                if (U[i - 1].Count == 0)
                {
                    i--;
                    q = l[i - 1];
                    updateU = false;
                }
                else
                {
                    int vi = vertices[i - 1];
                    int j = U[i - 1][0];
                    coloring[vi] = j;
                    U[i - 1].Remove(j);

                    if (j < k)
                    {
                        if (j > q)
                            q++;
                        
                        if (i == n)
                        {
                            // TODO: store the current solution (jak???)
                            k = q;
                            for (int c = 0; c < n; c++)
                                if (coloring[vertices[c]] == k)
                                {
                                    j = k;
                                    break;
                                }

                            i = j - 1;
                            q = k - 1;
                            updateU = false;
                        }
                        else
                        {
                            l[i - 1] = q;
                            i++;
                            updateU = true;
                        }
                    }
                    else
                    {
                        i--;
                        q = l[i - 1];
                        updateU = false;
                    }
                }
            }

            return (coloring.Max(), coloring);
        }
    }
}