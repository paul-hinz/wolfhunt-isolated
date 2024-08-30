using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Position = Mars.Interfaces.Environments.Position;
using Mars.Interfaces.Annotations;
// ReSharper disable All

namespace WolfHuntIsolated.Model;

public abstract class AbstractAnimal : IPositionable, IAgent<LandscapeLayer> {

    [ActiveConstructor]
    protected AbstractAnimal() {
    }
    
    
    protected AbstractAnimal(
        LandscapeLayer landscapeLayer, 
        Perimeter perimeter,
        Guid id,
        double latitude, 
        double longitude,
        Position position) { 
        Position = position;
        LandscapeLayer = landscapeLayer;
        Perimeter = perimeter;
        ID = id;
    }

    #region Properties and Fields

    protected double RunDistancePerTick = 0;
    protected bool IsFirstTick = true;
    public Guid ID { get; set; }
    public abstract Position Position { get; set; }
    public abstract Position Target { get; set; }
    public const double Distance = 5000.0;
    public LandscapeLayer LandscapeLayer { get; set; }
    public abstract double Latitude { get; set; }
    public abstract double Longitude { get; set; }
    public Perimeter Perimeter { get; set; }

    protected double TickLengthInSec = GlobalValueHelper.TickSeconds;
    
    
    #endregion

    #region Constants

    
    [PropertyDescription]
    public int RandomWalkMaxDistanceInM { get; set;  }
    
    [PropertyDescription]
    public int RandomWalkMinDistanceInM { get; set;  }
    
    protected const double MaxHydration = 100.0;
    protected const double MaxSatiety = 100.0; 
    public const int MaxAge = 25;

    #endregion
    
    public void Init(LandscapeLayer layer) {
        LandscapeLayer = layer;

        var spawnPosition = new Position(Longitude, Latitude);
        LandscapeLayer = layer;
        Position = Position.CreateGeoPosition(Longitude, Latitude);
    }
    
    public abstract void Tick();

    public abstract void FirstTick();

    public abstract void CalculateParams();

    protected double TicksToDays(int ticks)
    {
        return (ticks * TickLengthInSec) / (60 * 60 * 24);
    }

}
