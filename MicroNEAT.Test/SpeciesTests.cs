using MicroNEAT.Config;
using MicroNEAT.Core.Population;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class SpeciesTests
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
            FitnessFunction = new XOR(),
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            CompatibilityThreshold = 3.0,
            DropOffAge = 15,
            SurvivalRate = 0.2
        };
    }

    [Test]
    public void Species_Constructor_InitializesCorrectly()
    {
        var species = new Species(0, _config);

        Assert.That(species.Id, Is.EqualTo(0));
        Assert.That(species.Config, Is.EqualTo(_config));
        Assert.That(species.Genomes, Is.Empty);
        Assert.That(species.BestFitness, Is.EqualTo(0));
        Assert.That(species.GenerationsSinceImprovement, Is.EqualTo(0));
        Assert.That(species.Stagnated, Is.False);
    }

    [Test]
    public void Species_AddGenome_IncreasesGenomeCount()
    {
        var species = new Species(0, _config);
        var genome = GenomeBuilder.BuildGenome(_config);

        species.AddGenome(genome);

        Assert.That(species.Genomes.Count, Is.EqualTo(1));
        Assert.That(species.Genomes, Contains.Item(genome));
    }

    [Test]
    public void Species_SetRandomRepresentative_SetsRepresentative()
    {
        var species = new Species(0, _config);
        var genome = GenomeBuilder.BuildGenome(_config);
        species.AddGenome(genome);

        species.SetRandomRepresentative();

        Assert.That(species.Representative, Is.Not.Null);
        Assert.That(species.Genomes, Contains.Item(species.Representative));
    }

    [Test]
    public void Species_SetAdjustedFitness_CalculatesAdjustedFitness()
    {
        var species = new Species(0, _config);
        
        for (int i = 0; i < 5; i++)
        {
            var genome = GenomeBuilder.BuildGenome(_config);
            genome.Fitness = 1.0;
            species.AddGenome(genome);
        }

        species.SetAdjustedFitness();

        foreach (var genome in species.Genomes)
        {
            Assert.That(genome.AdjustedFitness, Is.GreaterThan(0));
        }
    }

    [Test]
    public void Species_GetTotalAdjustedFitness_ReturnsCorrectSum()
    {
        var species = new Species(0, _config);
        
        var genome1 = GenomeBuilder.BuildGenome(_config);
        genome1.AdjustedFitness = 0.5;
        var genome2 = GenomeBuilder.BuildGenome(_config);
        genome2.AdjustedFitness = 0.7;
        var genome3 = GenomeBuilder.BuildGenome(_config);
        genome3.AdjustedFitness = 0.8;
        
        species.AddGenome(genome1);
        species.AddGenome(genome2);
        species.AddGenome(genome3);

        var totalFitness = species.GetTotalAdjustedFitness();

        Assert.That(totalFitness, Is.EqualTo(2.0).Within(0.01));
    }

    [Test]
    public void Species_GetBestGenome_ReturnsHighestFitnessGenome()
    {
        var species = new Species(0, _config);
        
        var genome1 = GenomeBuilder.BuildGenome(_config);
        genome1.Fitness = 0.5;
        var genome2 = GenomeBuilder.BuildGenome(_config);
        genome2.Fitness = 0.9;
        var genome3 = GenomeBuilder.BuildGenome(_config);
        genome3.Fitness = 0.7;
        
        species.AddGenome(genome1);
        species.AddGenome(genome2);
        species.AddGenome(genome3);

        var bestGenome = species.GetBestGenome();

        Assert.That(bestGenome, Is.EqualTo(genome2));
        Assert.That(bestGenome.Fitness, Is.EqualTo(0.9));
    }

    [Test]
    public void Species_UpdateFitnessAndStagnation_UpdatesBestFitness()
    {
        var species = new Species(0, _config);
        
        var genome = GenomeBuilder.BuildGenome(_config);
        genome.Fitness = 0.8;
        species.AddGenome(genome);

        species.UpdateFitnessAndStagnation();

        Assert.That(species.BestFitness, Is.EqualTo(0.8));
        Assert.That(species.GenerationsSinceImprovement, Is.EqualTo(0));
    }

    [Test]
    public void Species_UpdateFitnessAndStagnation_DetectsStagnation()
    {
        var species = new Species(0, _config);
        
        var genome = GenomeBuilder.BuildGenome(_config);
        genome.Fitness = 0.5;
        species.AddGenome(genome);

        for (int i = 0; i <= _config.DropOffAge + 1; i++)
        {
            species.UpdateFitnessAndStagnation();
        }

        Assert.That(species.Stagnated, Is.True);
    }

    [Test]
    public void Species_RemoveBadGenomes_KeepsBestPerformers()
    {
        var species = new Species(0, _config);
        
        for (int i = 0; i < 10; i++)
        {
            var genome = GenomeBuilder.BuildGenome(_config);
            genome.Fitness = i * 0.1;
            species.AddGenome(genome);
        }

        species.RemoveBadGenomes();

        Assert.That(species.Genomes.Count, Is.LessThanOrEqualTo(10));
        Assert.That(species.Genomes.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(species.Genomes.First().Fitness, Is.GreaterThanOrEqualTo(species.Genomes.Last().Fitness));
    }
}
