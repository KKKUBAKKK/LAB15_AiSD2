﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            var coloring = new int[g.VertexCount];
            var bestColoring = new int[g.VertexCount];
            List<int> freeVertices = new List<int>();
            int[] freeColors = new int[g.VertexCount];
            int[] bestFreeColors = new int[g.VertexCount];
            int[] freeNeighbors = new int[g.VertexCount];
            var cutoffStack = new System.Collections.Generic.Stack<int>();
            var bestCutoffList = new List<int>();
            int numberOfColors = 1;
            int bestNumberOfColors = 1;
            for (int i = 0; i < g.VertexCount; i++)
            {
                if (g.Degree(i) > bestNumberOfColors)
                    bestNumberOfColors = g.Degree(i);
                
                freeVertices.Add(i);
                coloring[i] = -2;
                freeColors[i] = 1;
                freeNeighbors[i] = g.Degree(i);
            }
            bestNumberOfColors += 2;
            var takenColors = new bool[g.VertexCount, g.VertexCount];
            var bestTakenColors = new bool[g.VertexCount, g.VertexCount];
            
            FindBestColoringRec( g, numberOfColors,  ref bestNumberOfColors, cutoffStack, freeVertices, freeColors, 
                freeNeighbors, coloring, ref bestColoring, takenColors, ref bestTakenColors, ref bestFreeColors, 
                ref bestCutoffList);
            
            ColorCutoffVertices(g, bestCutoffList, ref bestNumberOfColors, ref bestTakenColors, ref bestColoring);
            
            return (bestNumberOfColors, bestColoring);
        }
        
        // Glowna funkcja rekurencyjna programu
        public void FindBestColoringRec(Graph g, int numberOfColors,  ref int bestNumberOfColors, 
            System.Collections.Generic.Stack<int> cutoffStack, List<int> freeVertices, int[] freeColors, 
            int[] freeNeighbors, int[] coloring, ref int[] bestColoring, bool[,] takenColors, ref bool[,] bestTakenColors,
            ref int[] bestFreeColors, ref List<int> bestCutoffList)
        {
            // TODO: implementacja
            if (numberOfColors >= bestNumberOfColors)
                return;
            
            int v = FindVertexToColor(ref coloring, freeVertices, ref freeColors, ref freeNeighbors, cutoffStack);
            
            if (freeVertices.Count == 0)
            {
                CheckColoring(ref numberOfColors, ref bestNumberOfColors, ref takenColors, ref bestTakenColors,
                    ref coloring, ref bestColoring, freeVertices, ref freeColors, ref bestFreeColors, cutoffStack, bestCutoffList);
                return;
            }
            
            if (v != -1)
            {
                RecursiveCall(ref v, ref numberOfColors, ref takenColors, ref freeVertices, ref freeColors, ref freeNeighbors, ref coloring, g, 
                    ref bestNumberOfColors, cutoffStack, ref bestColoring, ref bestTakenColors, ref bestFreeColors, bestCutoffList);

                RecursiveCallWithNewColor(ref v, ref numberOfColors, ref bestNumberOfColors, ref takenColors, freeVertices, ref freeColors,
                    ref freeNeighbors, ref coloring, g, cutoffStack, ref bestColoring, ref bestTakenColors, ref bestFreeColors, bestCutoffList);
            }

            while (cutoffStack.Count > 0)
                freeVertices.Add(cutoffStack.Pop());
        }
        
        // Wybor wierzcholka, ktory ma mniej dostepnych kolorow niz niepokolorowanych sasiadow i ma z takich
        // wierzcholkow najmniej dostepnych kolorow, a jesli tyle samo, to najwiecej niepokolorowanych sasiadow
        // uncolored, coloring, avcolors, avneighbors
        public int FindVertexToColor(ref int[] coloring, List<int> freeVertices, ref int[] freeColors, ref int[] freeNeighbors,
            System.Collections.Generic.Stack<int> cutoffStack)
        {
            int res = -1;
            for (int i = 0; i < freeVertices.Count; i++)
            {
                int v = freeVertices[i];
                if (freeNeighbors[v] < freeColors[v])
                {
                    freeVertices.Remove(v);
                    i--;
                    coloring[v] = -1;
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
            ref bool[,] takenColors, ref bool[,] bestTakenColors,
            ref int[] coloring, ref int[] bestColoring,
            List<int> freeVertices, 
            ref int[] freeColors, ref int[] bestFreeColors,
            System.Collections.Generic.Stack<int> cutoffStack, List<int> bestCutoffList)
        {

            if (numberOfColors < bestNumberOfColors)
            {
                bestCutoffList.Clear();
                bestCutoffList.AddRange(cutoffStack);
                bestNumberOfColors = numberOfColors;
                bestTakenColors = (bool[,])takenColors.Clone();
                bestColoring = (int[])coloring.Clone();
                bestFreeColors = (int[])freeColors.Clone();
            }
            
            while (cutoffStack.Count > 0)
                freeVertices.Add(cutoffStack.Pop());
        }
        
        // Funkcja wywoluje rekurencje na kazdym mozliwym kolorze, przed przygotowuje dane, a po przywraca je
        public void RecursiveCall(ref int v, ref int numberOfColors, ref bool[,] takenColors, ref List<int> freeVertices, 
            ref int[] freeColors, ref int[] freeNeighbors, ref int[] coloring, Graph g, ref int bestNumberOfColors, 
            System.Collections.Generic.Stack<int> cutoffStack, 
            ref int[] bestColoring, ref bool[,] bestTakenColors, ref int[] bestFreeColors, List<int> bestCutoffList)
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
                
                FindBestColoringRec( g, numberOfColors,  ref bestNumberOfColors, cutoffStack, freeVertices, freeColors, 
                    freeNeighbors, coloring, ref bestColoring, takenColors, ref bestTakenColors, ref bestFreeColors, ref bestCutoffList);

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

        // Funkcja dodaje nowy kolor jesli jest to mozliwe i wywoluje na nim rekurencje
        public void RecursiveCallWithNewColor(ref int v, ref int numberOfColors, ref int bestNumberOfColors, ref bool[,] takenColors, 
            List<int> freeVertices, ref int[] freeColors, ref int[] freeNeighbors, ref int[] coloring, Graph g, 
            System.Collections.Generic.Stack<int> cutoffStack, ref int[] bestColoring, ref bool[,] bestTakenColors, 
            ref int[] bestFreeColors, List<int> bestCutoffList)
        {
            if (numberOfColors + 1 >= bestNumberOfColors)
                return;

            freeVertices.Remove(v);
            numberOfColors++;
            coloring[v] = numberOfColors - 1;

            for (int i = 0; i < g.VertexCount; i++)
            {
                if (i != v)
                    freeColors[i]++;
            }

            foreach (var neighbor in g.OutNeighbors(v))
            {
                freeColors[neighbor]--;
                freeNeighbors[neighbor]--;
                takenColors[neighbor, numberOfColors - 1] = true;
            }
            
            FindBestColoringRec( g, numberOfColors,  ref bestNumberOfColors, cutoffStack, freeVertices, freeColors, 
            freeNeighbors, coloring, ref bestColoring, takenColors, ref bestTakenColors, ref bestFreeColors, ref bestCutoffList);

            foreach (var neighbor in g.OutNeighbors(v))
            {
                freeColors[neighbor]++;
                freeNeighbors[neighbor]++;
                takenColors[neighbor, numberOfColors - 1] = false;
            }

            for (int i = 0; i < g.VertexCount; i++)
            {
                if (i != v)
                    freeColors[i]--;
            }

            numberOfColors--;
            coloring[v] = -2;
            freeVertices.Add(v);
            
            while (cutoffStack.Count > 0)
                freeVertices.Add(cutoffStack.Pop());
        }
        
        // Funkcja koloruje wierzcholki pozostawione na koniec
        public void ColorCutoffVertices(Graph g, List<int> bestCutoffList, ref int bestNumberOfColors, ref bool[,] bestTakenColors, ref int[] bestColoring)
        {
            SafePriorityQueue<int, int> cutoffVerticesOrdered = new SafePriorityQueue<int, int>(new MyComparer(), bestCutoffList.Count);

            foreach (var v in bestCutoffList)
            {
                int freeColors = 0;
                
                for (int i = 0; i < bestNumberOfColors; i++)
                    if (!bestTakenColors[v, i])
                        freeColors++;
                
                cutoffVerticesOrdered.Insert(v, freeColors);
            }

            while (cutoffVerticesOrdered.Count > 0)
            {
                int v = cutoffVerticesOrdered.Extract();
                int c = -1;
                
                for (int i = 0; i <= bestNumberOfColors; i++)
                    if (!bestTakenColors[v, i])
                    {
                        c = i;
                        break;
                    }

                foreach (var neighbor in g.OutNeighbors(v))
                    bestTakenColors[neighbor, c] = true;

                bestColoring[v] = c;
            }
        }
        
        // Klasa do kolejki priorytetowej
        class MyComparer : Comparer<int>
        {
            public override int Compare(int x, int y)
            {
                return (x < y) ? -1 : 1;
            }
        }
    }
}