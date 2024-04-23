using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Item
{
    public int Weight { get; set; }
    public int Value { get; set; }

    public Item(int weight, int value)
    {
        Weight = weight;
        Value = value;
    }
}

public class GeneticAlgorithmKnapsack
{
    private List<Item> items;
    private int capacity;

    public GeneticAlgorithmKnapsack(List<Item> items, int capacity)
    {
        this.items = items;
        this.capacity = capacity;
    }

    public int[] Solve(int populationSize, double crossoverRate, double mutationRate, int numGenerations)
    {
        var population = GenerateInitialPopulation(items.Count, populationSize);

        for (int gen = 0; gen < numGenerations; gen++)
        {
            var nextGeneration = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                var parent1 = TournamentSelection(population);
                var parent2 = RouletteSelection(population);
                var offspring = Crossover(parent1, parent2, crossoverRate);
                Mutate(offspring, mutationRate);
                nextGeneration.Add(offspring);
            }
            population = nextGeneration;
        }

        var bestSolution = population.OrderByDescending(individual => FitnessFunction(individual)).First();
        return bestSolution;
    }

    private List<int[]> GenerateInitialPopulation(int size, int populationSize)
    {
        var population = new List<int[]>();
        var rand = new Random();
        for (int i = 0; i < populationSize; i++)
        {
            var individual = new int[size];
            for (int j = 0; j < size; j++)
            {
                individual[j] = rand.Next(2);
            }
            population.Add(individual);
        }
        return population;
    }

    private int FitnessFunction(int[] solution)
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

        if (totalWeight > capacity)
        {
            totalValue = 0;
        }
        return totalValue;
    }

    private int[] TournamentSelection(List<int[]> population)
    {
        var tournamentSize = 5;
        var rand = new Random();
        var tournament = population.OrderBy(x => rand.Next()).Take(tournamentSize).ToList();
        var bestSolution = tournament.OrderByDescending(individual => FitnessFunction(individual)).First();
        return bestSolution;
    }

    private int[] RouletteSelection(List<int[]> population)
    {
        var totalFitness = population.Sum(individual => FitnessFunction(individual));
        var randFitness = new Random().NextDouble() * totalFitness;
        var cumulativeFitness = 0;
        foreach (var individual in population)
        {
            cumulativeFitness += FitnessFunction(individual);
            if (cumulativeFitness >= randFitness)
            {
                return individual;
            }
        }
        return population.Last();
    }

    private int[] Crossover(int[] parent1, int[] parent2, double crossoverRate)
    {
        var rand = new Random();
        if (rand.NextDouble() > crossoverRate)
        {
            return rand.NextDouble() > 0.5 ? parent1.ToArray() : parent2.ToArray();
        }


        int crossoverPoint = rand.Next(1, parent1.Length);


        var offspring = new int[parent1.Length];
        Array.Copy(parent1, offspring, crossoverPoint);
        Array.Copy(parent2, crossoverPoint, offspring, crossoverPoint, parent2.Length - crossoverPoint);

        return offspring;
    }

    private void Mutate(int[] solution, double mutationRate)
    {
        var rand = new Random();
        for (int i = 0; i < solution.Length; i++)
        {
            if (rand.NextDouble() < mutationRate)
            {
                solution[i] = 1 - solution[i];
            }
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var items = ReadItemsFromFile("items.txt");
        int capacity = int.Parse(File.ReadAllText("capacity.txt"));


        int populationSize = 50;
        double crossoverRate = 0.8;
        double mutationRate = 0.1;
        int numGenerations = 500;


        var knapsack = new GeneticAlgorithmKnapsack(items, capacity);
        var solution = knapsack.Solve(populationSize, crossoverRate, mutationRate, numGenerations);


        Console.WriteLine("Itens selecionados:");
        for (int i = 0; i < items.Count; i++)
        {
            if (solution[i] == 1)
            {
                Console.WriteLine($"Item {i + 1}: Peso = {items[i].Weight}, Valor = {items[i].Value}");
            }
        }
    }

    private static List<Item> ReadItemsFromFile(string fileName)
    {
        var items = new List<Item>();
        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                int weight = int.Parse(values[0]);
                int value = int.Parse(values[1]);
                items.Add(new Item(weight, value));
            }
        }
        return items;
    }
}
