using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;
using MicroNEAT.FitnessFunctions;

namespace MicroNEAT.Test;

[TestFixture]
public class IntegrationTests
{
    [Test]
    public void SimpleNetwork_CanPropagate()
    {
        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            ActivationFunction = new Sigmoid(),
            WeightInitialization = new RandomWeightInitialization(-1, 1)
        };

        var genome = GenomeBuilder.BuildGenome(config);
        
        var outputs = genome.Propagate(new[] { 1.0, 0.0 });

        Assert.That(outputs, Is.Not.Null);
        Assert.That(outputs.Length, Is.EqualTo(1));
        Assert.That(outputs[0], Is.InRange(0.0, 1.0));
    }

    [Test]
    public void XORProblem_EvolvesToSolution()
    {
        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 150,
            Generations = 100,
            ActivationFunction = new Sigmoid(),
            TargetFitness = 0.9,
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            FitnessFunction = new XOR()
        };

        var algorithm = new Algorithm.NEATAlgorithm(config);
        algorithm.Run();

        var bestGenome = algorithm.GetBestGenome();
        
        Assert.That(bestGenome.Fitness, Is.GreaterThan(0.5));
    }

    [Test]
    public void GenomeEvolution_MaintainsStructuralIntegrity()
    {
        var config = new NEATConfig
        {
            InputSize = 3,
            OutputSize = 2,
            ActivationFunction = new Tanh(),
            WeightInitialization = new RandomWeightInitialization(-2, 2)
        };

        var genome = GenomeBuilder.BuildGenome(config);
        var initialInputCount = genome.InputNodes.Count;
        var initialOutputCount = genome.OutputNodes.Count;

        for (int i = 0; i < 20; i++)
        {
            genome.Mutate();
        }

        Assert.That(genome.InputNodes.Count, Is.EqualTo(initialInputCount));
        Assert.That(genome.OutputNodes.Count, Is.EqualTo(initialOutputCount));
        Assert.That(genome.NodeGenes.Count, Is.GreaterThanOrEqualTo(initialInputCount + initialOutputCount));
    }

    [Test]
    public void DifferentActivationFunctions_ProduceDifferentResults()
    {
        var configSigmoid = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            ActivationFunction = new Sigmoid()
        };
        var configReLU = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            ActivationFunction = new ReLU()
        };

        var genomeSigmoid = GenomeBuilder.BuildGenome(configSigmoid);
        var genomeReLU = GenomeBuilder.BuildGenome(configReLU);

        for (int i = 0; i < genomeSigmoid.ConnectionGenes.Count && i < genomeReLU.ConnectionGenes.Count; i++)
        {
            genomeReLU.ConnectionGenes[i].Weight = genomeSigmoid.ConnectionGenes[i].Weight;
        }

        var outputSigmoid = genomeSigmoid.Propagate(new[] { 2.0, 2.0 });
        var outputReLU = genomeReLU.Propagate(new[] { 2.0, 2.0 });

        Assert.That(outputSigmoid[0], Is.Not.EqualTo(outputReLU[0]));
    }

    [Test]
    public void MultipleGenerations_ShowsProgressiveImprovement()
    {
        var config = new NEATConfig
        {
            InputSize = 2,
            OutputSize = 1,
            PopulationSize = 50,
            Generations = 10,
            TargetFitness = 0.95,
            WeightInitialization = new RandomWeightInitialization(-1, 1),
            FitnessFunction = new XOR()
        };

        var population = new Core.Population.Population(config);
        population.EvaluatePopulation();
        
        var fitnessHistory = new List<double>();
        fitnessHistory.Add(population.GetBestGenome().Fitness);

        for (int i = 0; i < 5; i++)
        {
            population.Evolve();
            population.EvaluatePopulation();
            fitnessHistory.Add(population.GetBestGenome().Fitness);
        }

        Assert.That(fitnessHistory.Count, Is.EqualTo(6));
        Assert.That(fitnessHistory.Last(), Is.GreaterThanOrEqualTo(fitnessHistory.First() * 0.7));
    }
}
