using MicroNEAT.Algorithm;
using MicroNEAT.Config;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;

namespace MicroNEAT.Examples;

public class QuickXORTest
{
    public static void Run()
    {
        Console.WriteLine("Quick XOR Test with smaller population");
        Console.WriteLine("=======================================\n");

        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 50,
            Generations = 50,
            TargetFitness = 0.90,
            ActivationFunction = new Sigmoid(),
            FitnessFunction = new XOR(),
            WeightMutationRate = 0.8,
            AddConnectionMutationRate = 0.1,
            AddNodeMutationRate = 0.05
        };

        var algorithm = new NEATAlgorithm(config);
        algorithm.Run();

        var bestGenome = algorithm.GetBestGenome();
        Console.WriteLine($"\nBest Fitness: {bestGenome.Fitness:F6}");
        Console.WriteLine($"Nodes: {bestGenome.NodeGenes.Count}");
        Console.WriteLine($"Connections: {bestGenome.ConnectionGenes.Count}");
    }
}
