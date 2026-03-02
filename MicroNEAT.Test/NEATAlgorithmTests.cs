using MicroNEAT.Algorithm;
using MicroNEAT.Config;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class NEATAlgorithmTests
{
    private NEATConfig _config = null!;

    [SetUp]
    public void Setup()
    {
        _config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 50,
            Generations = 10,
            ActivationFunction = new Sigmoid(),
            FitnessFunction = new XOR(),
            TargetFitness = 0.95,
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };
    }

    [Test]
    public void NEATAlgorithm_Constructor_InitializesCorrectly()
    {
        var algorithm = new NEATAlgorithm(_config);
        var bestGenome = algorithm.GetBestGenome();

        Assert.That(bestGenome, Is.Not.Null);
    }

    [Test]
    public void NEATAlgorithm_GetBestGenome_ReturnsBestGenome()
    {
        var algorithm = new NEATAlgorithm(_config);
        
        var bestGenome = algorithm.GetBestGenome();

        Assert.That(bestGenome, Is.Not.Null);
        Assert.That(bestGenome.NodeGenes, Is.Not.Empty);
        Assert.That(bestGenome.ConnectionGenes, Is.Not.Empty);
    }

    [Test]
    public void NEATAlgorithm_Run_ExecutesEvolution()
    {
        _config.Generations = 3;
        var algorithm = new NEATAlgorithm(_config);
        var initialBestGenome = algorithm.GetBestGenome();
        var initialFitness = initialBestGenome.Fitness;

        algorithm.Run();

        var finalBestGenome = algorithm.GetBestGenome();
        Assert.That(finalBestGenome.Fitness, Is.GreaterThan(0));
    }

    [Test]
    public void NEATAlgorithm_Run_StopsAtTargetFitness()
    {
        _config.Generations = 1000;
        _config.TargetFitness = 0.0;
        _config.PopulationSize = 10;
        var algorithm = new NEATAlgorithm(_config);

        algorithm.Run();

        var bestGenome = algorithm.GetBestGenome();
        Assert.That(bestGenome.Fitness, Is.GreaterThanOrEqualTo(_config.TargetFitness));
    }
}
