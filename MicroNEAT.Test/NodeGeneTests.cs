using MicroNEAT.Config;
using MicroNEAT.Core.Genome.Genes.NodeGene;
using MicroNEAT.Core.Genome.Genes.ConnectionGene;
using MicroNEAT.ActivationFunctions;

namespace MicroNEAT.Test;

[TestFixture]
public class NodeGeneTests
{
    private NEATConfig _config = null!;

    [SetUp]
    public void Setup()
    {
        _config = new NEATConfig
        {
            ActivationFunction = new Sigmoid(),
            Bias = 1.0
        };
    }

    [Test]
    public void InputNode_Constructor_InitializesCorrectly()
    {
        var inputNode = new InputNode(0, _config);

        Assert.That(inputNode.Id, Is.EqualTo(0));
        Assert.That(inputNode.NodeType, Is.EqualTo(NodeType.INPUT));
        Assert.That(inputNode.Config, Is.EqualTo(_config));
        Assert.That(inputNode.LastOutput, Is.EqualTo(0));
        Assert.That(inputNode.AcceptsOutgoingConnections(), Is.True);
        Assert.That(inputNode.AcceptsIncomingConnections(), Is.False);
    }

    [Test]
    public void OutputNode_Constructor_InitializesCorrectly()
    {
        var outputNode = new OutputNode(1, _config);

        Assert.That(outputNode.Id, Is.EqualTo(1));
        Assert.That(outputNode.NodeType, Is.EqualTo(NodeType.OUTPUT));
        Assert.That(outputNode.Config, Is.EqualTo(_config));
        Assert.That(outputNode.LastOutput, Is.EqualTo(0));
        Assert.That(outputNode.AcceptsIncomingConnections(), Is.True);
        Assert.That(outputNode.AcceptsOutgoingConnections(), Is.False);
    }

    [Test]
    public void HiddenNode_Constructor_InitializesCorrectly()
    {
        var hiddenNode = new HiddenNode(2, _config);

        Assert.That(hiddenNode.Id, Is.EqualTo(2));
        Assert.That(hiddenNode.NodeType, Is.EqualTo(NodeType.HIDDEN));
        Assert.That(hiddenNode.Config, Is.EqualTo(_config));
        Assert.That(hiddenNode.AcceptsIncomingConnections(), Is.True);
        Assert.That(hiddenNode.AcceptsOutgoingConnections(), Is.True);
    }

    [Test]
    public void BiasNode_Constructor_InitializesCorrectly()
    {
        var biasNode = new BiasNode(3, _config);

        Assert.That(biasNode.Id, Is.EqualTo(3));
        Assert.That(biasNode.NodeType, Is.EqualTo(NodeType.BIAS));
        Assert.That(biasNode.Bias, Is.EqualTo(_config.Bias));
        Assert.That(biasNode.AcceptsOutgoingConnections(), Is.True);
        Assert.That(biasNode.AcceptsIncomingConnections(), Is.False);
    }

    [Test]
    public void InputNode_FeedInput_UpdatesLastOutput()
    {
        var inputNode = new InputNode(0, _config);
        var outputNode = new OutputNode(1, _config);
        var connection = new ConnectionGene(inputNode, outputNode, 1.0, true, 0, false, _config);
        
        inputNode.FeedInput(0.5);

        Assert.That(inputNode.LastOutput, Is.EqualTo(0.5));
    }

    [Test]
    public void NodeGene_ResetState_ResetsToDefaults()
    {
        var inputNode = new InputNode(0, _config);
        inputNode.FeedInput(0.5);
        inputNode.ExpectedInputs = 5;
        inputNode.ReceivedInputs = 3;

        inputNode.ResetState();

        Assert.That(inputNode.LastOutput, Is.EqualTo(0));
        Assert.That(inputNode.ExpectedInputs, Is.EqualTo(0));
        Assert.That(inputNode.ReceivedInputs, Is.EqualTo(0));
    }
}
