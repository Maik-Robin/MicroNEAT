using MicroNEAT.Util.Trackers;
using MicroNEAT.Config;

namespace MicroNEAT.Test;

[TestFixture]
public class TrackerTests
{
    [Test]
    public void InnovationTracker_TrackInnovation_AssignsUniqueNumbers()
    {
        var tracker = new InnovationTracker();

        var innovation1 = tracker.TrackInnovation(0, 1);
        var innovation2 = tracker.TrackInnovation(0, 2);
        var innovation3 = tracker.TrackInnovation(1, 2);

        Assert.That(innovation1.InnovationNumber, Is.Not.EqualTo(innovation2.InnovationNumber));
        Assert.That(innovation2.InnovationNumber, Is.Not.EqualTo(innovation3.InnovationNumber));
        Assert.That(innovation1.InnovationNumber, Is.Not.EqualTo(innovation3.InnovationNumber));
    }

    [Test]
    public void InnovationTracker_SameConnection_ReturnsSameInnovationNumber()
    {
        var tracker = new InnovationTracker();

        var innovation1 = tracker.TrackInnovation(0, 1);
        var innovation2 = tracker.TrackInnovation(0, 1);

        Assert.That(innovation1.InnovationNumber, Is.EqualTo(innovation2.InnovationNumber));
    }

    [Test]
    public void InnovationTracker_Reset_ClearsHistory()
    {
        var tracker = new InnovationTracker();
        
        var innovation1 = tracker.TrackInnovation(0, 1);
        var firstNumber = innovation1.InnovationNumber;
        
        tracker.Reset();
        
        var innovation2 = tracker.TrackInnovation(0, 1);

        Assert.That(innovation2.InnovationNumber, Is.Not.EqualTo(firstNumber));
    }

    [Test]
    public void NodeTracker_GetNextNodeId_IncrementsId()
    {
        var tracker = new NodeTracker();

        var id1 = tracker.GetNextNodeId();
        var id2 = tracker.GetNextNodeId();
        var id3 = tracker.GetNextNodeId();

        Assert.That(id2, Is.EqualTo(id1 + 1));
        Assert.That(id3, Is.EqualTo(id2 + 1));
    }

    [Test]
    public void GenomeTracker_GetNextGenomeId_IncrementsId()
    {
        var tracker = new GenomeTracker();

        var id1 = tracker.GetNextGenomeId();
        var id2 = tracker.GetNextGenomeId();
        var id3 = tracker.GetNextGenomeId();

        Assert.That(id2, Is.EqualTo(id1 + 1));
        Assert.That(id3, Is.EqualTo(id2 + 1));
    }

    [Test]
    public void PopulationTracker_GetNextPopulationId_IncrementsId()
    {
        var id1 = PopulationTracker.GetNextPopulationId();
        var id2 = PopulationTracker.GetNextPopulationId();

        Assert.That(id2, Is.GreaterThan(id1));
    }

    [Test]
    public void ConfigTracker_StoresConfig()
    {
        var config = new NEATConfig { InputSize = 2 };
        var tracker = new ConfigTracker(config);

        var retrievedConfig = tracker.GetConfig();

        Assert.That(retrievedConfig, Is.Not.Null);
        Assert.That(retrievedConfig.InputSize, Is.EqualTo(2));
    }

    [Test]
    public void ConfigTracker_DifferentConfigs_CreateDifferentTrackers()
    {
        var config1 = new NEATConfig { InputSize = 2 };
        var config2 = new NEATConfig { InputSize = 3 };

        var tracker1 = new ConfigTracker(config1);
        var tracker2 = new ConfigTracker(config2);

        Assert.That(tracker1.GetConfig().InputSize, Is.Not.EqualTo(tracker2.GetConfig().InputSize));
    }
}
