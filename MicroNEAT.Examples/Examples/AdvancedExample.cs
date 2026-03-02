using MicroNEAT.Algorithm;
using MicroNEAT.Config;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;
using MicroNEAT.Core.Genome;

namespace MicroNEAT.Examples;

/// <summary>
/// Advanced example demonstrating various NEAT features and configuration options
/// </summary>
public static class AdvancedExample
{
    public static void RunWithDifferentActivationFunctions()
    {
        Console.WriteLine("Testing Different Activation Functions");
        Console.WriteLine("======================================\n");

        var activationFunctions = new (string name, IActivationFunction function)[]
        {
            ("Sigmoid", new Sigmoid()),
            ("Tanh", new Tanh()),
            ("ReLU", new ReLU()),
            ("NEATSigmoid", new NEATSigmoid())
        };

        foreach (var (name, function) in activationFunctions)
        {
            Console.WriteLine($"\nTesting with {name}...");
            
            var config = new NEATConfig
            {
                InputSize = 2,
                OutputSize = 1,
                PopulationSize = 50,
                Generations = 30,
                TargetFitness = 0.90,
                ActivationFunction = function,
                FitnessFunction = new XOR()
            };

            var algorithm = new NEATAlgorithm(config);
            algorithm.Run();

            var best = algorithm.GetBestGenome();
            Console.WriteLine($"Final Fitness: {best.Fitness:F4}");
        }
    }

    public static void RunWithFluentConfiguration()
    {
        Console.WriteLine("Using Fluent Configuration API");
        Console.WriteLine("===============================\n");

        var config = new NEATConfig()
            .WithInputSize(2)
            .WithOutputSize(1)
            .WithPopulation(100)
            .WithGenerations(50)
            .WithTargetFitness(0.95)
            .WithActivationFunction(new Sigmoid())
            .WithFitnessFunction(new XOR())
            .WithMutationRates(weight: 0.8, addConnection: 0.1, addNode: 0.05);

        var algorithm = new NEATAlgorithm(config);
        algorithm.Run();
    }

    public static void AnalyzeEvolution()
    {
        Console.WriteLine("Evolution Analysis");
        Console.WriteLine("==================\n");

        var config = new NEATConfig
        {
            PopulationSize = 100,
            Generations = 100,
            TargetFitness = 0.95
        };

        var algorithm = new NEATAlgorithm(config);
        algorithm.Run();

        var best = algorithm.GetBestGenome();
        
        Console.WriteLine("\n--- Network Structure ---");
        Console.WriteLine($"Input Nodes: {best.InputNodes.Count}");
        Console.WriteLine($"Output Nodes: {best.OutputNodes.Count}");
        Console.WriteLine($"Hidden Nodes: {best.NodeGenes.Count - best.InputNodes.Count - best.OutputNodes.Count - (best.BiasNode != null ? 1 : 0)}");
        Console.WriteLine($"Total Nodes: {best.NodeGenes.Count}");
        Console.WriteLine($"Enabled Connections: {best.ConnectionGenes.Count(c => c.Enabled)}");
        Console.WriteLine($"Disabled Connections: {best.ConnectionGenes.Count(c => !c.Enabled)}");
        Console.WriteLine($"Recurrent Connections: {best.ConnectionGenes.Count(c => c.Recurrent)}");
        Console.WriteLine($"Final Fitness: {best.Fitness:F6}");

        Console.WriteLine("\n--- Testing Results ---");
        TestGenome(best);
    }

    private static void TestGenome(Genome genome)
    {
        var testCases = new[]
        {
            (input: new[] { 0.0, 0.0 }, expected: 0.0, label: "0 XOR 0"),
            (input: new[] { 0.0, 1.0 }, expected: 1.0, label: "0 XOR 1"),
            (input: new[] { 1.0, 0.0 }, expected: 1.0, label: "1 XOR 0"),
            (input: new[] { 1.0, 1.0 }, expected: 0.0, label: "1 XOR 1")
        };

        Console.WriteLine("Test Case\t\tOutput\t\tExpected\tError");
        Console.WriteLine("---------\t\t------\t\t--------\t-----");

        genome.ResetState();
        foreach (var (input, expected, label) in testCases)
        {
            var output = genome.Propagate(input);
            double error = Math.Abs(output[0] - expected);
            Console.WriteLine($"{label}\t\t{output[0]:F4}\t\t{expected:F1}\t\t{error:F4}");
        }
    }
}
