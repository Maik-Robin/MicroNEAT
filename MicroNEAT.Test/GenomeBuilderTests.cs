using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.Core.Genome.Genes.NodeGene;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class GenomeBuilderTests
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
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };
    }

    [Test]
    public void BuildGenome_CreatesGenomeWithCorrectNodeCount()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        var inputNodes = genome.NodeGenes.OfType<InputNode>().Count();
        var outputNodes = genome.NodeGenes.OfType<OutputNode>().Count();
        var biasNodes = genome.NodeGenes.OfType<BiasNode>().Count();

        Assert.That(inputNodes, Is.EqualTo(_config.InputSize));
        Assert.That(outputNodes, Is.EqualTo(_config.OutputSize));
        Assert.That(biasNodes, Is.EqualTo(1));
    }

    [Test]
    public void BuildGenome_CreatesInitialConnections()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        Assert.That(genome.ConnectionGenes, Is.Not.Empty);
        int expectedConnections = _config.InputSize * _config.OutputSize;
        if (_config.ConnectBias)
        {
            expectedConnections += _config.OutputSize;
        }

        Assert.That(genome.ConnectionGenes.Count, Is.EqualTo(expectedConnections));
    }

    [Test]
    public void BuildGenome_WithoutBias_DoesNotCreateBiasNode()
    {
        _config.BiasMode = "DISABLED";
        var genome = GenomeBuilder.BuildGenome(_config);

        var biasNodes = genome.NodeGenes.OfType<BiasNode>().Count();
        Assert.That(biasNodes, Is.EqualTo(0));
        Assert.That(genome.BiasNode, Is.Null);
    }

    [Test]
    public void BuildGenome_AssignsUniqueIds()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        var nodeIds = genome.NodeGenes.Select(n => n.Id).ToList();
        Assert.That(nodeIds.Distinct().Count(), Is.EqualTo(nodeIds.Count));
    }

    [Test]
    public void BuildGenome_AssignsInnovationNumbers()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        foreach (var connection in genome.ConnectionGenes)
        {
            Assert.That(connection.InnovationNumber, Is.GreaterThanOrEqualTo(0));
        }
    }
}
