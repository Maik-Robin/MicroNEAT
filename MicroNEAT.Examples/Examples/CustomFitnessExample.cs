using MicroNEAT.FitnessFunctions;
using MicroNEAT.Core.Genome;

namespace MicroNEAT.Examples;

public class CustomFitnessExample : IFitnessFunction
{
    public double CalculateFitness(Genome genome)
    {
        var testCases = new[]
        {
            (input: new[] { 0.0, 0.0 }, expected: 0.0),
            (input: new[] { 0.0, 1.0 }, expected: 1.0),
            (input: new[] { 1.0, 0.0 }, expected: 1.0),
            (input: new[] { 1.0, 1.0 }, expected: 0.0)
        };

        genome.ResetState();
        double totalError = 0;

        foreach (var (input, expected) in testCases)
        {
            var output = genome.Propagate(input);
            totalError += Math.Pow(output[0] - expected, 2);
        }

        return 1.0 / (1.0 + totalError);
    }
}
