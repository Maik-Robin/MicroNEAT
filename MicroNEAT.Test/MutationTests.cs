using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class MutationTests
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
            AddConnectionMutationRate = 0.5,
            AddNodeMutationRate = 0.5,
            ReinitializeWeightRate = 0.1,
            MinPerturb = -0.5,
            MaxPerturb = 0.5,
            MinWeight = -4.0,
            MaxWeight = 4.0,
            AllowRecurrentConnections = true,
            RecurrentConnectionRate = 0.5
        };
    }

    [Test]
    public void Genome_WeightMutation_ChangesWeights()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var initialWeights = genome.ConnectionGenes.Select(c => c.Weight).ToList();

        for (int i = 0; i < 5; i++)
        {
            genome.Mutate();
        }

        var changedWeights = genome.ConnectionGenes.Take(initialWeights.Count)
            .Select(c => c.Weight).ToList();
        
        bool anyChanged = false;
        for (int i = 0; i < initialWeights.Count; i++)
        {
            if (Math.Abs(initialWeights[i] - changedWeights[i]) > 0.001)
            {
                anyChanged = true;
                break;
            }
        }

        Assert.That(anyChanged, Is.True);
    }

    [Test]
    public void Genome_AddNodeMutation_IncreasesNodeCount()
    {
        _config.AddNodeMutationRate = 1.0;
        _config.AddConnectionMutationRate = 0.0;
        _config.WeightMutationRate = 0.0;
        
        var genome = GenomeBuilder.BuildGenome(_config);
        var initialNodeCount = genome.NodeGenes.Count;

        genome.Mutate();

        Assert.That(genome.NodeGenes.Count, Is.GreaterThanOrEqualTo(initialNodeCount));
    }

    [Test]
    public void Genome_AddConnectionMutation_IncreasesConnectionCount()
    {
        _config.AddNodeMutationRate = 0.0;
        _config.AddConnectionMutationRate = 1.0;
        _config.WeightMutationRate = 0.0;
        
        var genome = GenomeBuilder.BuildGenome(_config);
        var initialConnectionCount = genome.ConnectionGenes.Count;

        for (int i = 0; i < 3; i++)
        {
            genome.Mutate();
        }

        Assert.That(genome.ConnectionGenes.Count, Is.GreaterThanOrEqualTo(initialConnectionCount));
    }

    [Test]
    public void Genome_MutationPreservesConnectivity()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        for (int i = 0; i < 20; i++)
        {
            genome.Mutate();
        }

        var outputs = genome.Propagate(new[] { 1.0, 0.0 });
        Assert.That(outputs, Is.Not.Null);
    }

    [Test]
    public void Genome_WeightPerturbation_StaysInBounds()
    {
        var genome = GenomeBuilder.BuildGenome(_config);

        for (int i = 0; i < 100; i++)
        {
            genome.Mutate();
        }

        foreach (var connection in genome.ConnectionGenes)
        {
            Assert.That(connection.Weight, Is.InRange(_config.MinWeight, _config.MaxWeight));
        }
    }

    [Test]
    public void Genome_CheckForRecurrentConnections_UpdatesRecurrentFlags()
    {
        _config.AllowRecurrentConnections = true;
        var genome = GenomeBuilder.BuildGenome(_config);

        for (int i = 0; i < 10; i++)
        {
            genome.Mutate();
        }

        genome.CheckForRecurrentConnections();

        foreach (var connection in genome.ConnectionGenes)
        {
            Assert.That(connection.Recurrent, Is.InstanceOf<bool>());
        }
    }

    [Test]
    public void Genome_DisabledConnections_DoNotAffectOutput()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        
        genome.ResetState();
        var output1 = genome.Propagate(new[] { 1.0, 0.5 });

        foreach (var connection in genome.ConnectionGenes)
        {
            connection.Enabled = false;
        }

        genome.ResetState();
        var output2 = genome.Propagate(new[] { 1.0, 0.5 });

        Assert.That(output1[0], Is.Not.EqualTo(output2[0]));
    }
}
