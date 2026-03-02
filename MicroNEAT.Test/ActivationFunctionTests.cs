using MicroNEAT.ActivationFunctions;

namespace MicroNEAT.Test;

[TestFixture]
public class ActivationFunctionTests
{
    [Test]
    public void Sigmoid_AppliesCorrectly()
    {
        var sigmoid = new Sigmoid();
        
        Assert.That(sigmoid.Apply(0), Is.EqualTo(0.5).Within(0.0001));
        Assert.That(sigmoid.Apply(1), Is.GreaterThan(0.5));
        Assert.That(sigmoid.Apply(-1), Is.LessThan(0.5));
        Assert.That(sigmoid.Apply(100), Is.EqualTo(1.0).Within(0.0001));
        Assert.That(sigmoid.Apply(-100), Is.EqualTo(0.0).Within(0.0001));
    }

    [Test]
    public void ReLU_AppliesCorrectly()
    {
        var relu = new ReLU();
        
        Assert.That(relu.Apply(0), Is.EqualTo(0));
        Assert.That(relu.Apply(1), Is.EqualTo(1));
        Assert.That(relu.Apply(-1), Is.EqualTo(0));
        Assert.That(relu.Apply(5.5), Is.EqualTo(5.5));
        Assert.That(relu.Apply(-10), Is.EqualTo(0));
    }

    [Test]
    public void LeakyReLU_AppliesCorrectly()
    {
        var leakyRelu = new LeakyReLU(0.01);
        
        Assert.That(leakyRelu.Apply(0), Is.EqualTo(0));
        Assert.That(leakyRelu.Apply(1), Is.EqualTo(1));
        Assert.That(leakyRelu.Apply(-1), Is.EqualTo(-0.01).Within(0.0001));
        Assert.That(leakyRelu.Apply(5.5), Is.EqualTo(5.5));
        Assert.That(leakyRelu.Apply(-10), Is.EqualTo(-0.1).Within(0.0001));
    }

    [Test]
    public void LeakyReLU_CustomAlpha()
    {
        var leakyRelu = new LeakyReLU(0.1);
        
        Assert.That(leakyRelu.Apply(-1), Is.EqualTo(-0.1).Within(0.0001));
        Assert.That(leakyRelu.Apply(-10), Is.EqualTo(-1.0).Within(0.0001));
    }

    [Test]
    public void Tanh_AppliesCorrectly()
    {
        var tanh = new Tanh();
        
        Assert.That(tanh.Apply(0), Is.EqualTo(0).Within(0.0001));
        Assert.That(tanh.Apply(1), Is.GreaterThan(0));
        Assert.That(tanh.Apply(-1), Is.LessThan(0));
        Assert.That(tanh.Apply(100), Is.EqualTo(1.0).Within(0.0001));
        Assert.That(tanh.Apply(-100), Is.EqualTo(-1.0).Within(0.0001));
    }

    [Test]
    public void NEATSigmoid_AppliesCorrectly()
    {
        var neatSigmoid = new NEATSigmoid();
        
        Assert.That(neatSigmoid.Apply(0), Is.EqualTo(0.5).Within(0.0001));
        Assert.That(neatSigmoid.Apply(1), Is.GreaterThan(0.5));
        Assert.That(neatSigmoid.Apply(-1), Is.LessThan(0.5));
    }

    [Test]
    public void Gaussian_AppliesCorrectly()
    {
        var gaussian = new Gaussian();
        
        Assert.That(gaussian.Apply(0), Is.EqualTo(1.0).Within(0.0001));
        Assert.That(gaussian.Apply(1), Is.LessThan(1.0));
        Assert.That(gaussian.Apply(-1), Is.LessThan(1.0));
        Assert.That(gaussian.Apply(1), Is.EqualTo(gaussian.Apply(-1)).Within(0.0001));
    }

    [Test]
    public void SELU_AppliesCorrectly()
    {
        var selu = new SELU();
        
        Assert.That(selu.Apply(0), Is.EqualTo(0).Within(0.0001));
        Assert.That(selu.Apply(1), Is.GreaterThan(1));
        Assert.That(selu.Apply(-1), Is.LessThan(0));
    }
}
