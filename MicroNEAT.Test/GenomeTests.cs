using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.Core.Genome.Genes.NodeGene;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class GenomeTests
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
            BiasMode = "WEIGHTED_NODE",
            ConnectBias = true,
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            WeightMutationRate = 0.8,
            AddConnectionMutationRate = 0.05,
            AddNodeMutationRate = 0.03,
            ReinitializeWeightRate = 0.1,
            MinPerturb = -0.5,
            MaxPerturb = 0.5,
            MinWeight = -4.0,
            MaxWeight = 4.0
        };
    }

    [Test]
    public void Genome_Constructor_InitializesProperties()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        Assert.That(genome.NodeGenes, Is.Not.Empty);
        Assert.That(genome.ConnectionGenes, Is.Not.Empty);
        Assert.That(genome.InputNodes, Is.Not.Empty);
        Assert.That(genome.OutputNodes, Is.Not.Empty);
        Assert.That(genome.Config, Is.EqualTo(_config));
        Assert.That(genome.Id, Is.GreaterThanOrEqualTo(0));
        Assert.That(genome.Fitness, Is.EqualTo(0));
        Assert.That(genome.AdjustedFitness, Is.EqualTo(0));
    }

    [Test]
    public void Genome_Propagate_ReturnsCorrectOutputCount()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var inputs = new[] { 1.0, 0.0 };

        var outputs = genome.Propagate(inputs);

        Assert.That(outputs.Length, Is.EqualTo(_config.OutputSize));
    }

    [Test]
    public void Genome_Propagate_WithDifferentInputs_ProducesDifferentOutputs()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        
        genome.ResetState();
        var outputs1 = genome.Propagate(new[] { 0.0, 0.0 });
        
        genome.ResetState();
        var outputs2 = genome.Propagate(new[] { 1.0, 1.0 });

        Assert.That(outputs1[0], Is.Not.EqualTo(outputs2[0]));
    }

    [Test]
    public void Genome_ResetState_ResetsNodeStates()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        
        genome.Propagate(new[] { 1.0, 1.0 });
        genome.ResetState();

        foreach (var node in genome.NodeGenes)
        {
            Assert.That(node.LastOutput, Is.EqualTo(0));
            Assert.That(node.ReceivedInputs, Is.EqualTo(0));
        }
    }

    [Test]
    public void Genome_GetNodeById_ReturnsCorrectNode()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var firstNode = genome.NodeGenes.First();

        var retrievedNode = genome.GetNodeById(firstNode.Id);

        Assert.That(retrievedNode, Is.Not.Null);
        Assert.That(retrievedNode!.Id, Is.EqualTo(firstNode.Id));
    }

    [Test]
    public void Genome_GetNodeById_ReturnsNullForInvalidId()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        var retrievedNode = genome.GetNodeById(-9999);

        Assert.That(retrievedNode, Is.Null);
    }

    [Test]
    public void Genome_Mutate_ModifiesGenome()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var initialWeights = genome.ConnectionGenes.Select(c => c.Weight).ToList();
        var initialConnectionCount = genome.ConnectionGenes.Count;

        for (int i = 0; i < 10; i++)
        {
            genome.Mutate();
        }

        bool weightsChanged = !initialWeights.SequenceEqual(
            genome.ConnectionGenes.Take(initialWeights.Count).Select(c => c.Weight));
        bool structureChanged = genome.ConnectionGenes.Count != initialConnectionCount;

        Assert.That(weightsChanged || structureChanged, Is.True);
    }

    [Test]
    public void Genome_ReinitializeWeights_ChangesWeights()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        
        foreach (var connection in genome.ConnectionGenes)
        {
            connection.Weight = 0.5;
        }

        genome.ReinitializeWeights();

        bool anyDifferent = genome.ConnectionGenes.Any(c => Math.Abs(c.Weight - 0.5) > 0.001);
        Assert.That(anyDifferent, Is.True);
    }

    [Test]
    public void Genome_Copy_CreatesIndependentCopy()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var copy = genome.Copy();

        Assert.That(copy.Id, Is.Not.EqualTo(genome.Id));
        Assert.That(copy.NodeGenes.Count, Is.EqualTo(genome.NodeGenes.Count));
        Assert.That(copy.ConnectionGenes.Count, Is.EqualTo(genome.ConnectionGenes.Count));

        copy.ConnectionGenes.First().Weight = 99.0;
        Assert.That(genome.ConnectionGenes.First().Weight, Is.Not.EqualTo(99.0));
    }
}
