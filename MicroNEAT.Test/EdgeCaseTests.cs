using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.Core.Genome.Genes.NodeGene;
using MicroNEAT.Core.Genome.Genes.ConnectionGene;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;
using MicroNEAT.FitnessFunctions;

namespace MicroNEAT.Test;

[TestFixture]
public class EdgeCaseTests
{
    [Test]
    public void Genome_EmptyInputs_HandlesGracefully()
    {
        var config = new NEATConfig
        {
            InputSize = 0,
            OutputSize = 1,
            ActivationFunction = new Sigmoid(),
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };

        var genome = GenomeBuilder.BuildGenome(config);

        Assert.That(genome.InputNodes, Is.Empty);
        Assert.That(genome.OutputNodes, Is.Not.Empty);
    }

    [Test]
    public void Genome_MultipleOutputs_AllPopulated()
    {
        var config = new NEATConfig
        {
            InputSize = 3,
            OutputSize = 5,
            ActivationFunction = new ReLU(),
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };

        var genome = GenomeBuilder.BuildGenome(config);
        var outputs = genome.Propagate(new[] { 1.0, 2.0, 3.0 });

        Assert.That(outputs.Length, Is.EqualTo(5));
        Assert.That(outputs, Has.All.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void Genome_LargeNetwork_HandlesCorrectly()
    {
        var config = new NEATConfig
        {
            InputSize = 10,
            OutputSize = 5,
            ActivationFunction = new Tanh(),
            WeightInitialization = new RandomWeightInitialization(-2, 2)
        };

        var genome = GenomeBuilder.BuildGenome(config);

        Assert.That(genome.NodeGenes.Count, Is.GreaterThanOrEqualTo(15));
        Assert.That(genome.ConnectionGenes.Count, Is.GreaterThanOrEqualTo(50));
    }

    [Test]
    public void Genome_ExtremeWeights_StayWithinBounds()
    {
        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            MinWeight = -10.0,
            MaxWeight = 10.0,
            WeightMutationRate = 1.0,
            MinPerturb = -1.0,
            MaxPerturb = 1.0,
            WeightInitialization = new RandomWeightInitialization(-10, 10)
        };

        var genome = GenomeBuilder.BuildGenome(config);

        for (int i = 0; i < 50; i++)
        {
            genome.Mutate();
        }

        foreach (var connection in genome.ConnectionGenes)
        {
            Assert.That(connection.Weight, Is.InRange(config.MinWeight, config.MaxWeight));
        }
    }

    [Test]
    public void Population_VerySmall_StillWorks()
    {
        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 5,
            Generations = 3,
            FitnessFunction = new XOR()
        };

        var population = new Core.Population.Population(config);
        population.EvaluatePopulation();
        
        population.Evolve();
        population.EvaluatePopulation();

        Assert.That(population.Genomes.Count, Is.EqualTo(5));
    }

    [Test]
    public void Genome_MultipleResets_PreservesStructure()
    {
        var config = new NEATConfig();
        var genome = GenomeBuilder.BuildGenome(config);
        
        var nodeCount = genome.NodeGenes.Count;
        var connectionCount = genome.ConnectionGenes.Count;

        for (int i = 0; i < 10; i++)
        {
            genome.Propagate(new[] { 1.0, 1.0 });
            genome.ResetState();
        }

        Assert.That(genome.NodeGenes.Count, Is.EqualTo(nodeCount));
        Assert.That(genome.ConnectionGenes.Count, Is.EqualTo(connectionCount));
    }

    [Test]
    public void Genome_AllInputsZero_ProducesOutput()
    {
        var config = new NEATConfig();
        var genome = GenomeBuilder.BuildGenome(config);

        var outputs = genome.Propagate(new[] { 0.0, 0.0 });

        Assert.That(outputs, Is.Not.Null);
        Assert.That(outputs.Length, Is.GreaterThan(0));
    }

    [Test]
    public void Genome_NegativeInputs_HandlesCorrectly()
    {
        var config = new NEATConfig();
        var genome = GenomeBuilder.BuildGenome(config);

        var outputs = genome.Propagate(new[] { -5.0, -10.0 });

        Assert.That(outputs, Is.Not.Null);
        Assert.That(outputs.Length, Is.GreaterThan(0));
    }

    [Test]
    public void Species_EmptySpecies_HandlesGracefully()
    {
        var config = new NEATConfig();
        var species = new Core.Population.Species(0, config);

        species.SetRandomRepresentative();

        Assert.That(species.Representative, Is.Null);
        Assert.That(species.Genomes, Is.Empty);
    }

    [Test]
    public void ConnectionGene_ZeroWeight_StillFunctional()
    {
        var config = new NEATConfig();
        var inputNode = new InputNode(0, config);
        var outputNode = new OutputNode(1, config);
        
        var connection = new ConnectionGene(inputNode, outputNode, 0.0, true, 0, false, config);

        Assert.That(connection.Weight, Is.EqualTo(0.0));
        Assert.That(connection.Enabled, Is.True);
    }

    [Test]
    public void Genome_ConsecutivePropagations_ProducesConsistentResults()
    {
        var config = new NEATConfig();
        var genome = GenomeBuilder.BuildGenome(config);
        var inputs = new[] { 0.5, 0.5 };

        genome.ResetState();
        var output1 = genome.Propagate(inputs);
        
        genome.ResetState();
        var output2 = genome.Propagate(inputs);

        Assert.That(output1[0], Is.EqualTo(output2[0]).Within(0.00001));
    }
}
