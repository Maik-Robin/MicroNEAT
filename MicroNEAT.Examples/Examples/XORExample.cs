using MicroNEAT.Algorithm;
using MicroNEAT.Config;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;

namespace MicroNEAT.Examples;

public class XORExample
{
    public static void Run()
    {
        Console.WriteLine("NEAT-CSharp - XOR Problem Example");
        Console.WriteLine("===================================\n");

        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 150,
            Generations = 100,
            TargetFitness = 0.95,
            ActivationFunction = new Sigmoid(),
            FitnessFunction = new XOR(),
            BiasMode = "WEIGHTED_NODE",
            ConnectBias = true,
            AllowRecurrentConnections = true,
            WeightMutationRate = 0.8,
            AddConnectionMutationRate = 0.05,
            AddNodeMutationRate = 0.03,
            MutationRate = 1.0,
            SurvivalRate = 0.2,
            NumOfElite = 10
        };

        var algorithm = new NEATAlgorithm(config);
        
        Console.WriteLine("Starting evolution...\n");
        algorithm.Run();

        Console.WriteLine("\n--- Training Complete ---");
        var bestGenome = algorithm.GetBestGenome();
        Console.WriteLine($"Best Genome Fitness: {bestGenome.Fitness:F6}");
        Console.WriteLine($"Best Genome Nodes: {bestGenome.NodeGenes.Count}");
        Console.WriteLine($"Best Genome Connections: {bestGenome.ConnectionGenes.Count(c => c.Enabled)}");

        Console.WriteLine("\nTesting Best Genome on XOR Truth Table:");
        Console.WriteLine("Input\t\tOutput\t\tExpected");
        Console.WriteLine("-----\t\t------\t\t--------");
        
        var testCases = new[]
        {
            (input: new[] { 0.0, 0.0 }, expected: 0.0),
            (input: new[] { 0.0, 1.0 }, expected: 1.0),
            (input: new[] { 1.0, 0.0 }, expected: 1.0),
            (input: new[] { 1.0, 1.0 }, expected: 0.0)
        };

        bestGenome.ResetState();
        foreach (var (input, expected) in testCases)
        {
            var output = bestGenome.Propagate(input);
            //Console.WriteLine($"[{input[0]}, {input[1]}]\t\t{output[0]:F4}\t\t{expected:F1}");
            var outputBinary = output[0] >= 0.5 ? 1.0 : 0.0;
            Console.WriteLine($"[{input[0]}, {input[1]}]\t\t{outputBinary:F1}\t\t{expected:F1}");
        }
    }
}
