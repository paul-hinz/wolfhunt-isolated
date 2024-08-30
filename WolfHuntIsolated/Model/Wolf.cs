using System;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace WolfHuntIsolated.Model; 

public class Wolf : AbstractAnimal
{
    [ActiveConstructor]
    public Wolf()
    {
    }

    private const bool Logger = true;
    
    [ActiveConstructor] 
    public Wolf(
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
            position)
    {
    }

    #region Properties and Fields
    
    private bool IsFirstTick = true;
    
    public override Position Position { get; set; }
    public override Position Target { get; set; }
    [PropertyDescription(Name = "Latitude")]
    public override double Latitude { get; set; }
    [PropertyDescription(Name = "Longitude")]
    public override double Longitude { get; set; }

    public Prey HuntingTarget = null;
    
    #endregion

    #region Constants
    
    [PropertyDescription]
    
    //distance is per hour
    public static double MaxHuntDistanceInM { get; set; }

    [PropertyDescription] 
    public static double RunningSpeedInMs { get; set; }
    
    #endregion
    
    public override void Tick()
    {

        if (IsFirstTick)
        {
            FirstTick();
            IsFirstTick = false;
            return;
        }
        
        
    }

    public override void FirstTick()
    {
        CalculateParams();
    }

    public override void CalculateParams()
    {
        //Calculating Movements per Tick
        RunDistancePerTick = RunningSpeedInMs * TickLengthInSec;
        RandomWalkMaxDistanceInM = (int)Math.Round ((RandomWalkMaxDistanceInM / (double) 3600) * TickLengthInSec);
        RandomWalkMinDistanceInM = (int)Math.Round ((RandomWalkMinDistanceInM / (double) 3600) * TickLengthInSec);
    }
}