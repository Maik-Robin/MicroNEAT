using MicroNEAT.Config;
using MicroNEAT.Core.Population;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class PopulationTests
{
    private NEATConfig _config = null!;

    [SetUp]
    public void Setup()
    {
        _config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 20,
            ActivationFunction = new Sigmoid(),
            FitnessFunction = new XOR(),
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            CompatibilityThreshold = 3.0,
            SurvivalRate = 0.2,
            NumOfElite = 2,
            PopulationStagnationLimit = 20
        };
    }

    [Test]
    public void Population_Constructor_CreatesInitialPopulation()
    {
        var population = new Population(_config);

        Assert.That(population.Genomes.Count, Is.EqualTo(_config.PopulationSize));
        Assert.That(population.Config, Is.EqualTo(_config));
        Assert.That(population.Generation, Is.EqualTo(0));
        Assert.That(population.BestFitness, Is.EqualTo(0));
    }

    [Test]
    public void Population_EvaluatePopulation_CalculatesFitness()
    {
        var population = new Population(_config);

        population.EvaluatePopulation();

        foreach (var genome in population.Genomes)
        {
            Assert.That(genome.Fitness, Is.GreaterThan(0));
        }
    }

    [Test]
    public void Population_GetBestGenome_ReturnsHighestFitness()
    {
        var population = new Population(_config);
        population.EvaluatePopulation();

        var bestGenome = population.GetBestGenome();

        Assert.That(bestGenome, Is.Not.Null);
        foreach (var genome in population.Genomes)
        {
            Assert.That(bestGenome.Fitness, Is.GreaterThanOrEqualTo(genome.Fitness));
        }
    }

    [Test]
    public void Population_Speciate_CreatesSpecies()
    {
        var population = new Population(_config);
        population.EvaluatePopulation();

        population.Speciate();

        Assert.That(population.Species, Is.Not.Empty);
        Assert.That(population.Species.Sum(s => s.Genomes.Count), Is.EqualTo(population.Genomes.Count));
    }

    [Test]
    public void Population_Evolve_IncrementsGeneration()
    {
        var population = new Population(_config);
        population.EvaluatePopulation();
        var initialGeneration = population.Generation;

        population.Evolve();

        Assert.That(population.Generation, Is.EqualTo(initialGeneration + 1));
    }

    [Test]
    public void Population_Evolve_MaintainsPopulationSize()
    {
        var population = new Population(_config);
        population.EvaluatePopulation();

        population.Evolve();
        population.EvaluatePopulation();

        Assert.That(population.Genomes.Count, Is.EqualTo(_config.PopulationSize));
    }

    [Test]
    public void Population_MultipleGenerations_ImproveFitness()
    {
        _config.Generations = 5;
        var population = new Population(_config);
        population.EvaluatePopulation();
        
        var initialBestFitness = population.GetBestGenome().Fitness;

        for (int i = 0; i < 5; i++)
        {
            population.Evolve();
            population.EvaluatePopulation();
        }

        var finalBestFitness = population.GetBestGenome().Fitness;

        Assert.That(finalBestFitness, Is.GreaterThanOrEqualTo(initialBestFitness * 0.8));
    }
}
