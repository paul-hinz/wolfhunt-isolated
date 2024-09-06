using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace WolfHuntIsolated.Model;

public class LandscapeLayer : AbstractLayer {
    
    #region Properties and Fields
    
    public GeoHashEnvironment<AbstractAnimal> Environment { get; set; }

    private List<Prey> Preys { get; set; }
    private List<Wolf> Wolfs { get; set; }
    [PropertyDescription(Name = "Perimeter")]
    public Perimeter Fence { get; set; }

    private RegisterAgent _registerAgent;
    private UnregisterAgent _unregisterAgent;

    #endregion
    
    /// <summary>
    /// The LandscapeLayer registers the animals in the runtime system. In this way, the tick methods
    /// of the agents can be executed later. Then the expansion of the simulation area is calculated using
    /// the raster layers described in config.json. An environment is created with this bounding box.
    /// </summary>
    /// <param name="layerInitData"></param>
    /// <param name="registerAgentHandle"></param>
    /// <param name="unregisterAgentHandle"></param>
    /// <returns>true if the agents where registered</returns>
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle) {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        _registerAgent = registerAgentHandle;
        _unregisterAgent = unregisterAgentHandle;

        // Calculate and expand extent
        // Console.WriteLine(Fence is null);
        // var baseExtent = new Envelope(Fence.Extent.ToEnvelope());
        // Console.WriteLine(new BoundingBox(baseExtent));

        // Create GeoHashEnvironment with the calculated extent
        Environment = GeoHashEnvironment<AbstractAnimal>.BuildByBBox(new BoundingBox(53, -13, 54, -12), 1);

        var agentManager = layerInitData.Container.Resolve<IAgentManager>();
        Preys = agentManager.Spawn<Prey, LandscapeLayer>().ToList();
        Wolfs = agentManager.Spawn<Wolf, LandscapeLayer>().ToList();
        
        foreach (var prey in Preys)
        {
            Environment.Insert(prey);
        }
        
        foreach (var wolf in Wolfs)
        {
            Environment.Insert(wolf);
        }
        
        return Preys.Count + Wolfs.Count > 0;
    }

    public void SpawnPrey(LandscapeLayer landscapeLayer, Perimeter perimeter, double latitude, double longitude, Position position) {
        var newPrey = new Prey(landscapeLayer, perimeter,
            Guid.NewGuid(), latitude, longitude, position);
        Preys.Add(newPrey);
        Environment.Insert(newPrey);
        _registerAgent(landscapeLayer, newPrey);
    }
    
    public Wolf SpawnWolf(LandscapeLayer landscapeLayer, Perimeter perimeter, 
        double latitude, double longitude, Position position) {
        var newWolf = new Wolf(landscapeLayer, perimeter, Guid.NewGuid(), latitude, longitude, position);
        Wolfs.Add(newWolf);
        Environment.Insert(newWolf);
        _registerAgent(landscapeLayer, newWolf);
        return newWolf;
    }

    public void RemoveAnimal(LandscapeLayer landscapeLayer, AbstractAnimal animal) {
        _unregisterAgent(landscapeLayer, animal);
        if (animal is Prey prey)
        {
            Preys.Remove(prey);
        }
        else if (animal is Wolf wolf)
        {
            Wolfs.Remove(wolf);
        }
        Environment.Remove(animal);
    }
}
