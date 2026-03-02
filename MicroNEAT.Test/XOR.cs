namespace MicroNEAT.FitnessFunctions;

/// <summary>
/// Implements the XOR (exclusive OR) fitness function.
/// A classic test problem for neural networks that requires non-linear decision boundaries.
/// </summary>
public class XOR : IFitnessFunction
{
    /// <summary>
    /// Calculates the fitness of a genome on the XOR problem.
    /// Tests the genome on all four XOR input combinations and measures error.
    /// </summary>
    /// <param name="genome">The genome to evaluate.</param>
    /// <returns>A fitness score between 0 and 1, where 1 is perfect performance.</returns>
    public double CalculateFitness(Core.Genome.Genome genome)
    {
        var inputs = new[]
        {
            new[] { 0.0, 0.0 },
            new[] { 0.0, 1.0 },
            new[] { 1.0, 0.0 },
            new[] { 1.0, 1.0 }
        };
        var expectedOutputs = new[] { 0.0, 1.0, 1.0, 0.0 };
        
        genome.ResetState();
        double error = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            var output = genome.Propagate(inputs[i]);
            error += Math.Pow(output[0] - expectedOutputs[i], 2);
        }
        
        return 1.0 / (1.0 + error);
    }
}
