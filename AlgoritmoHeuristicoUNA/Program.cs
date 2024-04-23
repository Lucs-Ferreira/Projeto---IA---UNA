using System;
using System.Collections.Generic;
using System.IO;

class Item
{
    public int Weight { get; }
    public int Value { get; }

    public Item(int weight, int value)
    {
        Weight = weight;
        Value = value;
    }
}

class KnapsackProblem
{
    private List<Item> items;
    private int capacity;

    public KnapsackProblem(List<Item> items, int capacity)
    {
        this.items = items;
        this.capacity = capacity;
    }

    public void SolveTabuSearch(int tabuSize, int maxIterations)
    {
        int[] solution = TabuSearchKnapsack(tabuSize, maxIterations);

        Console.WriteLine("Itens selecionados:");
        for (int i = 0; i < items.Count; i++)
        {
            if (solution[i] == 1)
            {
                Console.WriteLine($"Item {i + 1}: Peso = {items[i].Weight}, Valor = {items[i].Value}");
            }
        }
    }

    private int[] TabuSearchKnapsack(int tabuSize, int maxIterations)
    {
        int[] currentSolution = GenerateRandomSolution(items.Count);
        int[] bestSolution = (int[])currentSolution.Clone();
        int currentCost = CostFunction(currentSolution);
        int bestCost = currentCost;
        List<int[]> tabuList = new List<int[]>();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            List<int[]> neighbors = GenerateNeighbors(currentSolution);
            int[] nextSolution = null;
            int nextCost = int.MinValue;

            foreach (int[] neighbor in neighbors)
            {
                if (!IsTabu(neighbor, tabuList))
                {
                    int neighborCost = CostFunction(neighbor);
                    if (neighborCost > nextCost)
                    {
                        nextSolution = neighbor;
                        nextCost = neighborCost;
                    }
                }
            }

            if (nextSolution == null)
            {
                break;
            }

            currentSolution = nextSolution;
            currentCost = nextCost;

            if (currentCost > bestCost)
            {
                bestSolution = (int[])currentSolution.Clone();
                bestCost = currentCost;
            }

            tabuList.Add(nextSolution);
            if (tabuList.Count > tabuSize)
            {
                tabuList.RemoveAt(0);
            }
        }

        return bestSolution;
    }

    private int[] GenerateRandomSolution(int size)
    {
        int[] solution = new int[size];
        Random rand = new Random();
        for (int i = 0; i < size; i++)
        {
            solution[i] = rand.Next(2); // 0 ou 1 (selecionado ou não selecionado)
        }
        return solution;
    }

    private int CostFunction(int[] solution)
    {
        int totalValue = 0;
        int totalWeight = 0;
        for (int i = 0; i < solution.Length; i++)
        {
            if (solution[i] == 1)
            {
                totalValue += items[i].Value;
                totalWeight += items[i].Weight;
            }
        }
        // Penalize soluções que excedam a capacidade da mochila
        if (totalWeight > capacity)
        {
            totalValue = 0;
        }
        return totalValue;
    }

    private List<int[]> GenerateNeighbors(int[] solution)
    {
        List<int[]> neighbors = new List<int[]>();

        for (int i = 0; i < solution.Length; i++)
        {
            int[] neighbor = (int[])solution.Clone();
            neighbor[i] = 1 - neighbor[i]; // Troca 0 por 1 e vice-versa
            neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private bool IsTabu(int[] solution, List<int[]> tabuList)
    {
        foreach (int[] tabuSolution in tabuList)
        {
            if (ArraysEqual(solution, tabuSolution))
            {
                return true;
            }
        }
        return false;
    }

    private bool ArraysEqual(int[] array1, int[] array2)
    {
        if (array1.Length != array2.Length)
        {
            return false;
        }
        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
            {
                return false;
            }
        }
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Ler itens dos arquivos
        List<Item> items = ReadItemsFromFile("items.txt");
        int capacity = ReadCapacityFromFile("capacity.txt");

        // Parâmetros da Busca Tabu
        int tabuSize = 5;
        int maxIterations = 1000;

        // Criar instância do problema da mochila e resolver usando busca tabu
        KnapsackProblem problem = new KnapsackProblem(items, capacity);
        problem.SolveTabuSearch(tabuSize, maxIterations);
    }

    static List<Item> ReadItemsFromFile(string filename)
    {
        List<Item> items = new List<Item>();
        string[] lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            int weight = int.Parse(parts[0]);
            int value = int.Parse(parts[1]);
            items.Add(new Item(weight, value));
        }
        return items;
    }

    static int ReadCapacityFromFile(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        return int.Parse(lines[0]);
    }
}