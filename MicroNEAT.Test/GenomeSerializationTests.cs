using MicroNEAT.Config;
using MicroNEAT.Core.Genome;
using MicroNEAT.ActivationFunctions;
using MicroNEAT.WeightInitialization;
using System.Text.Json;

namespace MicroNEAT.Test;

[TestFixture]
public class GenomeSerializationTests
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
            BiasMode = "WEIGHTED_NODE",
            ConnectBias = true
        };
    }

    [Test]
    public void Genome_ToJson_ReturnsValidJson()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        
        var json = genome.ToJson();

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.Not.Empty);
        Assert.That(json, Does.Contain("nodeGenes"));
        Assert.That(json, Does.Contain("connectionGenes"));
        Assert.That(json, Does.Contain("fitness"));
        Assert.That(json, Does.Contain("populationId"));
    }

    [Test]
    public void Genome_ToJson_ContainsAllNodes()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var nodeCount = genome.NodeGenes.Count;
        
        var json = genome.ToJson();

        var inputCount = json.Split("\"type\":").Length - 1;
        Assert.That(inputCount, Is.EqualTo(nodeCount));
    }

    [Test]
    public void Genome_ToJson_ContainsAllConnections()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var connectionCount = genome.ConnectionGenes.Count;
        
        var json = genome.ToJson();

        var innovationCount = json.Split("\"innovationNumber\":").Length - 1;
        Assert.That(innovationCount, Is.EqualTo(connectionCount));
    }

    [Test]
    public void GenomeBuilder_LoadGenome_ReconstructsGenome()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        originalGenome.Fitness = 0.85;
        
        var json = originalGenome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);

        Assert.That(loadedGenome, Is.Not.Null);
        Assert.That(loadedGenome.NodeGenes.Count, Is.EqualTo(originalGenome.NodeGenes.Count));
        Assert.That(loadedGenome.ConnectionGenes.Count, Is.EqualTo(originalGenome.ConnectionGenes.Count));
        Assert.That(loadedGenome.Fitness, Is.EqualTo(originalGenome.Fitness));
        Assert.That(loadedGenome.PopulationId, Is.EqualTo(originalGenome.PopulationId));
    }

    [Test]
    public void GenomeBuilder_LoadGenome_PreservesWeights()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        
        var json = originalGenome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);

        for (int i = 0; i < originalGenome.ConnectionGenes.Count; i++)
        {
            Assert.That(loadedGenome.ConnectionGenes[i].Weight, 
                Is.EqualTo(originalGenome.ConnectionGenes[i].Weight).Within(0.0001));
        }
    }

    [Test]
    public void GenomeBuilder_LoadGenome_PreservesNodeTypes()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        
        var json = originalGenome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);

        for (int i = 0; i < originalGenome.NodeGenes.Count; i++)
        {
            Assert.That(loadedGenome.NodeGenes[i].NodeType, 
                Is.EqualTo(originalGenome.NodeGenes[i].NodeType));
        }
    }

    [Test]
    public void GenomeBuilder_LoadGenome_PreservesConnectionState()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        originalGenome.ConnectionGenes[0].Enabled = false;
        
        var json = originalGenome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);

        Assert.That(loadedGenome.ConnectionGenes[0].Enabled, Is.False);
    }

    [Test]
    public void GenomeBuilder_LoadGenome_PreservesRecurrentFlag()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        originalGenome.Mutate();
        originalGenome.CheckForRecurrentConnections();
        
        var json = originalGenome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);

        for (int i = 0; i < originalGenome.ConnectionGenes.Count; i++)
        {
            Assert.That(loadedGenome.ConnectionGenes[i].Recurrent, 
                Is.EqualTo(originalGenome.ConnectionGenes[i].Recurrent));
        }
    }

    [Test]
    public void Genome_RoundTripSerialization_ProducesSameOutput()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        var inputs = new[] { 1.0, 0.5 };
        
        originalGenome.ResetState();
        var originalOutputs = originalGenome.Propagate(inputs);
        
        var json = originalGenome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);
        
        loadedGenome.ResetState();
        var loadedOutputs = loadedGenome.Propagate(inputs);

        for (int i = 0; i < originalOutputs.Length; i++)
        {
            Assert.That(loadedOutputs[i], Is.EqualTo(originalOutputs[i]).Within(0.0001));
        }
    }

    [Test]
    public void GenomeBuilder_LoadGenome_WithInvalidJson_ThrowsException()
    {
        var invalidJson = "{ invalid json }";

        Assert.Throws<JsonException>(() => GenomeBuilder.LoadGenome(invalidJson, _config));
    }

    [Test]
    public void GenomeBuilder_LoadGenome_WithMissingNode_ThrowsException()
    {
        var invalidJson = @"{
            ""id"": 0,
            ""nodeGenes"": [{""id"": 0, ""type"": ""INPUT""}],
            ""connectionGenes"": [{
                ""innovationNumber"": 0,
                ""inNodeId"": 0,
                ""outNodeId"": 99,
                ""enabled"": true,
                ""weight"": 1.0,
                ""recurrent"": false
            }],
            ""fitness"": 0.0,
            ""populationId"": 0
        }";

        Assert.Throws<InvalidOperationException>(() => GenomeBuilder.LoadGenome(invalidJson, _config));
    }

    [Test]
    public void Genome_SerializeComplexGenome_PreservesStructure()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        
        for (int i = 0; i < 10; i++)
        {
            genome.Mutate();
        }
        
        genome.Fitness = 0.92;
        
        var json = genome.ToJson();
        var loadedGenome = GenomeBuilder.LoadGenome(json, _config);

        Assert.That(loadedGenome.NodeGenes.Count, Is.EqualTo(genome.NodeGenes.Count));
        Assert.That(loadedGenome.ConnectionGenes.Count, Is.EqualTo(genome.ConnectionGenes.Count));
        Assert.That(loadedGenome.Fitness, Is.EqualTo(genome.Fitness));
    }

    [Test]
    public void GenomeBuilder_SaveGenome_CreatesFile()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var testFile = "test_genome.json";

        try
        {
            GenomeBuilder.SaveGenome(genome, testFile);

            Assert.That(File.Exists(testFile), Is.True);
        }
        finally
        {
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }

    [Test]
    public void GenomeBuilder_LoadGenomeFromFile_LoadsSuccessfully()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        genome.Fitness = 0.88;
        var testFile = "test_load_genome.json";

        try
        {
            GenomeBuilder.SaveGenome(genome, testFile);
            var loadedGenome = GenomeBuilder.LoadGenomeFromFile(testFile, _config);

            Assert.That(loadedGenome, Is.Not.Null);
            Assert.That(loadedGenome.Fitness, Is.EqualTo(genome.Fitness));
            Assert.That(loadedGenome.NodeGenes.Count, Is.EqualTo(genome.NodeGenes.Count));
        }
        finally
        {
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }

    [Test]
    public void GenomeBuilder_SaveAndLoad_RoundTrip()
    {
        var originalGenome = GenomeBuilder.BuildGenome(_config);
        var testFile = "roundtrip_genome.json";

        try
        {
            GenomeBuilder.SaveGenome(originalGenome, testFile);
            var loadedGenome = GenomeBuilder.LoadGenomeFromFile(testFile, _config);

            var inputs = new[] { 1.0, 0.5 };
            
            originalGenome.ResetState();
            var originalOutputs = originalGenome.Propagate(inputs);
            
            loadedGenome.ResetState();
            var loadedOutputs = loadedGenome.Propagate(inputs);

            Assert.That(loadedOutputs[0], Is.EqualTo(originalOutputs[0]).Within(0.0001));
        }
        finally
        {
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }

    [Test]
    public void GenomeBuilder_LoadGenomeFromFile_NonExistentFile_ThrowsException()
    {
        var nonExistentFile = "this_file_does_not_exist.json";

        Assert.Throws<FileNotFoundException>(() => 
            GenomeBuilder.LoadGenomeFromFile(nonExistentFile, _config));
    }

    [Test]
    public void GenomeBuilder_SaveGenome_WithComplexGenome()
    {
        var genome = GenomeBuilder.BuildGenome(_config);
        var testFile = "complex_genome.json";

        for (int i = 0; i < 15; i++)
        {
            genome.Mutate();
        }

        try
        {
            GenomeBuilder.SaveGenome(genome, testFile);
            var loadedGenome = GenomeBuilder.LoadGenomeFromFile(testFile, _config);

            Assert.That(loadedGenome.NodeGenes.Count, Is.EqualTo(genome.NodeGenes.Count));
            Assert.That(loadedGenome.ConnectionGenes.Count, Is.EqualTo(genome.ConnectionGenes.Count));
        }
        finally
        {
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }

    [Test]
    public void GenomeBuilder_SaveGenome_OverwritesExistingFile()
    {
        var genome1 = GenomeBuilder.BuildGenome(_config);
        genome1.Fitness = 0.5;
        
        var genome2 = GenomeBuilder.BuildGenome(_config);
        genome2.Fitness = 0.9;
        
        var testFile = "overwrite_test.json";

        try
        {
            GenomeBuilder.SaveGenome(genome1, testFile);
            GenomeBuilder.SaveGenome(genome2, testFile);
            
            var loadedGenome = GenomeBuilder.LoadGenomeFromFile(testFile, _config);

            Assert.That(loadedGenome.Fitness, Is.EqualTo(genome2.Fitness));
        }
        finally
        {
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }
}
