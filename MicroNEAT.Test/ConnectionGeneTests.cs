using MicroNEAT.Config;
using MicroNEAT.Core.Genome.Genes.NodeGene;
using MicroNEAT.Core.Genome.Genes.ConnectionGene;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class ConnectionGeneTests
{
    private NEATConfig _config = null!;
    private InputNode _inputNode = null!;
    private OutputNode _outputNode = null!;

    [SetUp]
    public void Setup()
    {
        _config = new NEATConfig
        {
            ActivationFunction = new Sigmoid(),
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            MinWeight = -4.0,
            MaxWeight = 4.0
        };
        _inputNode = new InputNode(0, _config);
        _outputNode = new OutputNode(1, _config);
    }

    [Test]
    public void ConnectionGene_Constructor_InitializesProperties()
    {
        var connection = new ConnectionGene(_inputNode, _outputNode, 0.5, true, 0, false, _config);

        Assert.That(connection.InNode, Is.EqualTo(_inputNode));
        Assert.That(connection.OutNode, Is.EqualTo(_outputNode));
        Assert.That(connection.Weight, Is.EqualTo(0.5));
        Assert.That(connection.Enabled, Is.True);
        Assert.That(connection.InnovationNumber, Is.EqualTo(0));
        Assert.That(connection.Recurrent, Is.False);
        Assert.That(connection.Config, Is.EqualTo(_config));
    }

    [Test]
    public void ConnectionGene_Disabled_DoesNotFeedForward()
    {
        var connection = new ConnectionGene(_inputNode, _outputNode, 1.0, false, 0, false, _config);
        _outputNode.ExpectedInputs = 1;

        connection.FeedForward(1.0);

        Assert.That(_outputNode.ReceivedInputs, Is.EqualTo(0));
    }

    [Test]
    public void ConnectionGene_ReinitializeWeight_ChangesWeight()
    {
        var connection = new ConnectionGene(_inputNode, _outputNode, 0.5, true, 0, false, _config);
        var originalWeight = connection.Weight;

        bool changed = false;
        for (int i = 0; i < 10; i++)
        {
            connection.ReinitializeWeight();
            if (Math.Abs(connection.Weight - originalWeight) > 0.001)
            {
                changed = true;
                break;
            }
        }

        Assert.That(changed, Is.True);
        Assert.That(connection.Weight, Is.InRange(_config.MinWeight, _config.MaxWeight));
    }

    [Test]
    public void ConnectionGene_RecurrentConnection_DoesNotFeedForward()
    {
        var connection = new ConnectionGene(_inputNode, _outputNode, 1.0, true, 0, true, _config);
        _outputNode.ExpectedInputs = 1;

        connection.FeedForward(1.0);

        Assert.That(_outputNode.ReceivedInputs, Is.EqualTo(0));
    }
}
