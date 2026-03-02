using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.Core.Genome.Genes.GeneticEncoding;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;
using MicroNEAT.FitnessFunctions;

namespace MicroNEAT.Test;

[TestFixture]
public class GeneticOperationTests
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
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            WeightMutationRate = 1.0,
            AddConnectionMutationRate = 0.1,
            AddNodeMutationRate = 0.1,
            ReinitializeWeightRate = 0.1,
            MinPerturb = -0.5,
            MaxPerturb = 0.5,
            MinWeight = -4.0,
            MaxWeight = 4.0,
            KeepDisabledOnCrossOverRate = 0.75,
            C1 = 1.0,
            C2 = 1.0,
            C3 = 0.4,
            FitnessFunction = new XOR()
        };
    }

    [Test]
    public void Genome_Crossover_ProducesOffspring()
    {
        var parent1 = GenomeBuilder.BuildGenome(_config);
        var parent2 = GenomeBuilder.BuildGenome(_config);
        
        parent1.Fitness = 0.8;
        parent2.Fitness = 0.6;

        var offspring = parent1.Crossover(parent2);

        Assert.That(offspring, Is.Not.Null);
        Assert.That(offspring.NodeGenes, Is.Not.Empty);
        Assert.That(offspring.ConnectionGenes, Is.Not.Empty);
    }

    [Test]
    public void Genome_Crossover_OffspringIsIndependent()
    {
        var parent1 = GenomeBuilder.BuildGenome(_config);
        var parent2 = GenomeBuilder.BuildGenome(_config);

        var offspring = parent1.Crossover(parent2);

        offspring.ConnectionGenes.First().Weight = 99.0;

        Assert.That(parent1.ConnectionGenes.First().Weight, Is.Not.EqualTo(99.0));
        Assert.That(parent2.ConnectionGenes.First().Weight, Is.Not.EqualTo(99.0));
    }

    [Test]
    public void Genome_GetGeneticEncoding_ReturnsEncoding()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        var encoding = genome.GetGeneticEncoding();

        Assert.That(encoding, Is.Not.Null);
        Assert.That(encoding, Is.InstanceOf<GeneticEncoding>());
    }

    [Test]
    public void GeneticEncoding_CalculateCompatibilityDistance_ReturnsSameForIdenticalGenomes()
    {
        var genome1 = GenomeBuilder.BuildGenome(_config);
        var genome2 = genome1.Copy();

        var encoding1 = genome1.GetGeneticEncoding();
        var encoding2 = genome2.GetGeneticEncoding();

        var distance = encoding1.CalculateCompatibilityDistance(encoding2);

        Assert.That(distance, Is.LessThan(0.1));
    }

    [Test]
    public void GeneticEncoding_CalculateCompatibilityDistance_IncreasesWithDifferences()
    {
        var genome1 = GenomeBuilder.BuildGenome(_config);
        var genome2 = GenomeBuilder.BuildGenome(_config);

        for (int i = 0; i < 5; i++)
        {
            genome2.Mutate();
        }

        var encoding1 = genome1.GetGeneticEncoding();
        var encoding2 = genome2.GetGeneticEncoding();

        var distance = encoding1.CalculateCompatibilityDistance(encoding2);

        Assert.That(distance, Is.GreaterThan(0));
    }

    [Test]
    public void Genome_EvaluateFitness_SetsFitnessValue()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        genome.Fitness = 0;

        genome.EvaluateFitness();

        Assert.That(genome.Fitness, Is.GreaterThan(0));
    }

    [Test]
    public void Genome_EqualsGenome_ReturnsTrueForIdenticalGenomes()
    {
        var genome1 = GenomeBuilder.BuildGenome(_config);
        var genome2 = genome1.Copy();

        foreach (var i in Enumerable.Range(0, genome1.ConnectionGenes.Count))
        {
            genome2.ConnectionGenes[i].Weight = genome1.ConnectionGenes[i].Weight;
        }

        var equals = genome1.EqualsGenome(genome2);

        Assert.That(equals, Is.True);
    }

    [Test]
    public void Genome_EqualsGenome_ReturnsFalseForDifferentGenomes()
    {
        var genome1 = GenomeBuilder.BuildGenome(_config);
        var genome2 = GenomeBuilder.BuildGenome(_config);
        genome2.ConnectionGenes.First().Weight = 99.0;

        var equals = genome1.EqualsGenome(genome2);

        Assert.That(equals, Is.False);
    }
}
