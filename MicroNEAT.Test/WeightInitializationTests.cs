using MicroNEAT.WeightInitialization;

namespace MicroNEAT.Test;

[TestFixture]
public class WeightInitializationTests
{
    [Test]
    public void RandomWeightInitialization_Constructor_SetsMinMax()
    {
        var weightInit = new RandomWeightInitialization(-2.0, 2.0);

        Assert.That(weightInit, Is.Not.Null);
    }

    [Test]
    public void RandomWeightInitialization_InitializeWeight_ReturnsValueInRange()
    {
        var weightInit = new RandomWeightInitialization(-2.0, 2.0);

        for (int i = 0; i < 100; i++)
        {
            var weight = weightInit.InitializeWeight();
            Assert.That(weight, Is.InRange(-2.0, 2.0));
        }
    }

    [Test]
    public void RandomWeightInitialization_InitializeWeights_ReturnsCorrectSize()
    {
        var weightInit = new RandomWeightInitialization(-1.0, 1.0);
        
        var weights = weightInit.InitializeWeights(10);

        Assert.That(weights.Length, Is.EqualTo(10));
    }

    [Test]
    public void RandomWeightInitialization_InitializeWeights_AllInRange()
    {
        var weightInit = new RandomWeightInitialization(-3.0, 3.0);
        
        var weights = weightInit.InitializeWeights(50);

        foreach (var weight in weights)
        {
            Assert.That(weight, Is.InRange(-3.0, 3.0));
        }
    }

    [Test]
    public void RandomWeightInitialization_ProducesVariedWeights()
    {
        var weightInit = new RandomWeightInitialization(-5.0, 5.0);
        
        var weights = weightInit.InitializeWeights(100);
        var distinctWeights = weights.Distinct().Count();

        Assert.That(distinctWeights, Is.GreaterThan(90));
    }

    [Test]
    public void RandomWeightInitialization_ZeroRange_ReturnsConstant()
    {
        var weightInit = new RandomWeightInitialization(1.5, 1.5);
        
        var weight = weightInit.InitializeWeight();

        Assert.That(weight, Is.EqualTo(1.5).Within(0.0001));
    }
}
