using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;
using MicroNEAT.Algorithm;

namespace MicroNEAT.Examples;

/// <summary>
/// Demonstrates how to save and load genomes using JSON serialization.
/// This allows you to persist trained networks and reload them later.
/// </summary>
public static class SaveLoadExample
{
    public static void Run()
    {
        Console.WriteLine("=== Genome Save/Load Example ===\n");

        // Create and configure NEAT
        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 50,
            Generations = 50,
            TargetFitness = 0.95,
            ActivationFunction = new Sigmoid(),
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };

        // Train a genome
        Console.WriteLine("Training genome on XOR problem...");
        var algorithm = new NEATAlgorithm(config);
        algorithm.Run();

        var trainedGenome = algorithm.GetBestGenome();
        Console.WriteLine($"\nTrained genome fitness: {trainedGenome.Fitness:F6}");
        Console.WriteLine($"Nodes: {trainedGenome.NodeGenes.Count}");
        Console.WriteLine($"Connections: {trainedGenome.ConnectionGenes.Count}");

        // Save the genome to JSON
        var jsonData = trainedGenome.ToJson();
        Console.WriteLine("\n--- Saved Genome JSON ---");
        Console.WriteLine(jsonData);

        // Save to file using convenience method
        var filename = "trained_genome.json";
        GenomeBuilder.SaveGenome(trainedGenome, filename);
        Console.WriteLine($"\nGenome saved to {filename}");

        // Load the genome from file using convenience method
        Console.WriteLine("\nLoading genome from file...");
        var loadedGenome = GenomeBuilder.LoadGenomeFromFile(filename, config);

        Console.WriteLine($"Loaded genome fitness: {loadedGenome.Fitness:F6}");
        Console.WriteLine($"Nodes: {loadedGenome.NodeGenes.Count}");
        Console.WriteLine($"Connections: {loadedGenome.ConnectionGenes.Count}");

        // Test that loaded genome produces same results
        Console.WriteLine("\n=== Testing Loaded Genome ===");
        TestGenome(loadedGenome);

        Console.WriteLine("\n=== Comparing Original and Loaded Outputs ===");
        var testInputs = new[]
        {
            new[] { 0.0, 0.0 },
            new[] { 0.0, 1.0 },
            new[] { 1.0, 0.0 },
            new[] { 1.0, 1.0 }
        };

        foreach (var input in testInputs)
        {
            trainedGenome.ResetState();
            var originalOutput = trainedGenome.Propagate(input);
            
            loadedGenome.ResetState();
            var loadedOutput = loadedGenome.Propagate(input);

            Console.WriteLine($"Input: [{input[0]}, {input[1]}]");
            Console.WriteLine($"  Original output: {originalOutput[0]:F6}");
            Console.WriteLine($"  Loaded output:   {loadedOutput[0]:F6}");
            Console.WriteLine($"  Difference:      {Math.Abs(originalOutput[0] - loadedOutput[0]):F8}");
        }

        // Clean up
        if (File.Exists(filename))
        {
            File.Delete(filename);
            Console.WriteLine($"\n{filename} deleted.");
        }
    }

    private static void TestGenome(Genome genome)
    {
        var testCases = new[]
        {
            new { Input = new[] { 0.0, 0.0 }, Expected = 0.0 },
            new { Input = new[] { 0.0, 1.0 }, Expected = 1.0 },
            new { Input = new[] { 1.0, 0.0 }, Expected = 1.0 },
            new { Input = new[] { 1.0, 1.0 }, Expected = 0.0 }
        };

        Console.WriteLine("XOR Test Results:");
        foreach (var testCase in testCases)
        {
            genome.ResetState();
            var output = genome.Propagate(testCase.Input);
            var error = Math.Abs(output[0] - testCase.Expected);
            Console.WriteLine($"  Input: [{testCase.Input[0]}, {testCase.Input[1]}] => " +
                            $"Output: {output[0]:F4}, Expected: {testCase.Expected:F1}, Error: {error:F4}");
        }
    }
}
