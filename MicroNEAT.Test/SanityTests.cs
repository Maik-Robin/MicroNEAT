using MicroNEAT.Config;
using MicroNEAT.Core.Genome;

namespace MicroNEAT.Test;

[TestFixture]
public class SanityTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SanityCheck_BasicGenomeCreation_Succeeds()
    {
        var config = new NEATConfig();
        var genome = GenomeBuilder.BuildGenome(config);

        Assert.That(genome, Is.Not.Null);
        Assert.That(genome.NodeGenes, Is.Not.Empty);
        Assert.That(genome.ConnectionGenes, Is.Not.Empty);
    }

    [Test]
    public void SanityCheck_BasicPropagation_Succeeds()
    {
        var config = new NEATConfig();
        var genome = GenomeBuilder.BuildGenome(config);

        var outputs = genome.Propagate(new[] { 1.0, 0.0 });

        Assert.That(outputs, Is.Not.Null);
        Assert.That(outputs.Length, Is.GreaterThan(0));
    }
}
