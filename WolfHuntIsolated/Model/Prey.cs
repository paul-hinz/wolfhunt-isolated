using System;
using Position = Mars.Interfaces.Environments.Position;
using Mars.Interfaces.Annotations;

namespace WolfHuntIsolated.Model;

public class Prey : AbstractAnimal {

    [ActiveConstructor]
    public Prey() {
    }
    
    [ActiveConstructor]
    public Prey(
        LandscapeLayer landscapeLayer, 
        Perimeter perimeter,
        Guid id,
        double latitude, 
        double longitude,
        Position position) : 
        base(landscapeLayer, 
        perimeter,
        id,
        latitude, 
        longitude,
        position) { 
    }
    
    #region Properties and Fields
    
    public override Position Position { get; set; }
    public override Position Target { get; set; }
    [PropertyDescription(Name = "Latitude")]
    public override double Latitude { get; set; }
    [PropertyDescription(Name = "Longitude")]
    public override double Longitude { get; set; }
        
    #endregion
    
    #region Constants
    
    [PropertyDescription] 
    public static double RunningSpeedInMs { get; set; }
    #endregion
    
    public override void Tick() {
        
        if (IsFirstTick)
        {
            FirstTick();
            IsFirstTick = false;
            return;
        }
    }
    public override void FirstTick()
    {
        Console.WriteLine("Prey spawned at:" + Position);
        CalculateParams();
    }

    public override void CalculateParams()
    {
        //Calculating Movements per Tick
        RunDistancePerTick = RunningSpeedInMs * TickLengthInSec;
        RandomWalkMaxDistanceInM = (int)Math.Round ((RandomWalkMaxDistanceInM / (double) 3600) * TickLengthInSec);
        RandomWalkMinDistanceInM = (int)Math.Round((RandomWalkMinDistanceInM / (double)3600) * TickLengthInSec);
    } 
}
