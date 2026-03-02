using MicroNEAT.Config;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.FitnessFunctions;
using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class NEATConfigTests
{
    [Test]
    public void NEATConfig_DefaultConstructor_SetsDefaultValues()
    {
        var config = new NEATConfig();

        Assert.That(config.InputSize, Is.EqualTo(2));
        Assert.That(config.OutputSize, Is.EqualTo(1));
        Assert.That(config.ActivationFunction, Is.InstanceOf<Sigmoid>());
        Assert.That(config.Bias, Is.EqualTo(1.0));
        Assert.That(config.ConnectBias, Is.True);
        Assert.That(config.BiasMode, Is.EqualTo("WEIGHTED_NODE"));
        Assert.That(config.AllowRecurrentConnections, Is.True);
        Assert.That(config.RecurrentConnectionRate, Is.EqualTo(1.0));
        Assert.That(config.MinWeight, Is.EqualTo(-4.0));
        Assert.That(config.MaxWeight, Is.EqualTo(4.0));
        Assert.That(config.WeightInitialization, Is.InstanceOf<RandomWeightInitialization>());
        Assert.That(config.PopulationSize, Is.EqualTo(150));
        Assert.That(config.Generations, Is.EqualTo(100));
        Assert.That(config.TargetFitness, Is.EqualTo(0.95));
    }

    [Test]
    public void NEATConfig_CustomValues_CanBeSet()
    {
        var config = new NEATConfig
        {
            InputSize = 4,
            OutputSize = 2,
            PopulationSize = 200,
            Generations = 50,
            TargetFitness = 0.99
        };

        Assert.That(config.InputSize, Is.EqualTo(4));
        Assert.That(config.OutputSize, Is.EqualTo(2));
        Assert.That(config.PopulationSize, Is.EqualTo(200));
        Assert.That(config.Generations, Is.EqualTo(50));
        Assert.That(config.TargetFitness, Is.EqualTo(0.99));
    }

    [Test]
    public void NEATConfig_MutationRates_AreWithinValidRange()
    {
        var config = new NEATConfig();

        Assert.That(config.MutationRate, Is.InRange(0.0, 1.0));
        Assert.That(config.WeightMutationRate, Is.InRange(0.0, 1.0));
        Assert.That(config.AddConnectionMutationRate, Is.InRange(0.0, 1.0));
        Assert.That(config.AddNodeMutationRate, Is.InRange(0.0, 1.0));
        Assert.That(config.ReinitializeWeightRate, Is.InRange(0.0, 1.0));
    }

    [Test]
    public void NEATConfig_WeightBounds_AreValid()
    {
        var config = new NEATConfig();

        Assert.That(config.MinWeight, Is.LessThan(config.MaxWeight));
        Assert.That(config.MinPerturb, Is.LessThan(config.MaxPerturb));
    }
}
