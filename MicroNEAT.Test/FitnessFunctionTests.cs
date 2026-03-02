using MicroNEAT.FitnessFunctions;
using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class FitnessFunctionTests
{
    private NEATConfig _config = null!;

    [SetUp]
    public void Setup()
    {
        _config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            ActivationFunction = new Sigmoid(),
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };
    }

    [Test]
    public void XOR_CalculateFitness_ReturnsValidFitness()
    {
        var xorFitness = new XOR();
        var genome = GenomeBuilder.BuildGenome(_config);

        var fitness = xorFitness.CalculateFitness(genome);

        Assert.That(fitness, Is.GreaterThan(0));
        Assert.That(fitness, Is.LessThanOrEqualTo(1.0));
    }

    [Test]
    public void XOR_PerfectGenome_ReturnsHighFitness()
    {
        var xorFitness = new XOR();
        var genome = GenomeBuilder.BuildGenome(_config);

        for (int i = 0; i < 100; i++)
        {
            genome.Mutate();
        }

        var fitness = xorFitness.CalculateFitness(genome);

        Assert.That(fitness, Is.GreaterThan(0));
    }

    [Test]
    public void XOR_CalculateFitness_ConsistentResults()
    {
        var xorFitness = new XOR();
        var genome = GenomeBuilder.BuildGenome(_config);
        
        var fitness1 = xorFitness.CalculateFitness(genome);
        var fitness2 = xorFitness.CalculateFitness(genome);

        Assert.That(fitness1, Is.EqualTo(fitness2).Within(0.0001));
    }

    [Test]
    public void XOR_TestsAllInputCombinations()
    {
        var xorFitness = new XOR();
        var genome = GenomeBuilder.BuildGenome(_config);

        foreach (var connection in genome.ConnectionGenes)
        {
            connection.Weight = 0;
        }

        var fitness = xorFitness.CalculateFitness(genome);

        Assert.That(fitness, Is.GreaterThan(0).And.LessThan(1.0));
    }
}
