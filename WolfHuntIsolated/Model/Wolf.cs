using System;
using System.IO;
using Mars.Common;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace WolfHuntIsolated.Model; 

public class Wolf : AbstractAnimal, IComparable
{
    [ActiveConstructor]
    public Wolf()
    {
    }
    
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
    
    public override Position Position { get; set; }
    public override Position Target { get; set; }
    [PropertyDescription(Name = "Latitude")]
    public override double Latitude { get; set; }
    [PropertyDescription(Name = "Longitude")]
    public override double Longitude { get; set; }

    public Prey HuntingTarget = null;
    private bool IsTickZero = true;
    public double BearingToPrey;
    
    #endregion

    #region Constants
    
    [PropertyDescription]
    
    //distance is per hour
    public static double MaxHuntDistanceInM { get; set; }

    [PropertyDescription] 
    public static double RunningSpeedInMs { get; set; }
    
    [PropertyDescription] 
    public static double SafeDistanceToPrey { get; set; }
    
    #endregion
    
    public override void Tick()
    {
        if (IsTickZero)
        {
            IsTickZero = false;
            return;
        }
        if (IsFirstTick)
        {
            FirstTick();
            IsFirstTick = false;
            return;
        }

        CalculateTarget();
        
        Position = Target;
        WolfLister._list.UpdateList();
    }

    public override void FirstTick()
    {
        CalculateParams();
        HuntingTarget = (Prey) LandscapeLayer.Environment.GetNearest(Position, 10000.0, IsPrey);
        //Console.WriteLine("Hunting Target set to:" + HuntingTarget);
        if (HuntingTarget is null) throw new IOException("Defined Prey Spawnpoint is too far away");
        var startDist = Position.DistanceInMTo(HuntingTarget.Position);
        if (startDist < SafeDistanceToPrey)
        {
            Console.WriteLine("Wolf spawned too close" + startDist + "is less than" + SafeDistanceToPrey);
            WolfLister._list.Register(this);
        }
    }

    public override void CalculateParams()
    {
        //Calculating Movements per Tick
        RunDistancePerTick = RunningSpeedInMs * TickLengthInSec;
        RandomWalkMaxDistanceInM = (int)Math.Round ((RandomWalkMaxDistanceInM / (double) 3600) * TickLengthInSec);
        RandomWalkMinDistanceInM = (int)Math.Round ((RandomWalkMinDistanceInM / (double) 3600) * TickLengthInSec);
    }
    
    private static bool IsPrey(AbstractAnimal animal)
    {
        return animal is Prey;
    }
    
    private void CalculateTarget()
    {
        var distLeft = RunDistancePerTick;
        if (Position.DistanceInMTo(HuntingTarget.Position) > SafeDistanceToPrey)
        {
            //catch up to safe distance
            var bearing = Position.GetBearing(HuntingTarget.Position);
            Target = Position.GetRelativePosition(bearing, RunDistancePerTick);

            var newDist = Target.DistanceInMTo(HuntingTarget.Position);

            // if too close move away
            if (newDist > SafeDistanceToPrey) return;
            Target = HuntingTarget.Position.GetRelativePosition(InvertBearing(bearing), SafeDistanceToPrey);
            BearingToPrey = bearing;
            WolfLister._list.Register(this);
            distLeft = SafeDistanceToPrey - newDist;
        }

        //find spot on circle
        var left = WolfLister._list.GetLeft(this);
        var right = WolfLister._list.GetRight(this);

        if (left is not null)
        {
            if (right is null)
            {
                var oppositeBearing = left.Position.GetBearing(HuntingTarget.Position);
                Target = HuntingTarget.Position.GetRelativePosition(oppositeBearing, SafeDistanceToPrey);
                BearingToPrey = InvertBearing(oppositeBearing);
                return;
            }
            
                //Find middle between both neighbours
                var fullDist = left.Position.DistanceInMTo(right.Position);
                var bear = left.Position.GetBearing(right.Position);
                var middle = left.Position.GetRelativePosition(bear, fullDist / 2);

                // check if swap is needed
                var bearDiffLeft = HuntingTarget.Position.GetBearing(left.Position) -
                                   Position.GetBearing(left.Position);
                var bearDiffRight = HuntingTarget.Position.GetBearing(right.Position) -
                                    Position.GetBearing(right.Position);

                //Calculate bearing
                var bearingFromPrey = middle.GetBearing(HuntingTarget.Position);

                //invert if all three wolfs are on one half of the circle
                if (Math.Abs(bearDiffRight) + Math.Abs(bearDiffLeft) < 180) bearingFromPrey = InvertBearing(bearingFromPrey);

                Target = HuntingTarget.Position.GetRelativePosition(bearingFromPrey, SafeDistanceToPrey);

                var distToTarget = Position.DistanceInMTo(Target);
                if (distToTarget > distLeft)
                    Target = Position.GetRelativePosition(Position.GetBearing(Target), distLeft);

                BearingToPrey = InvertBearing(bearingFromPrey);
        }
    }

    public int CompareTo(object obj)
    {
        if (obj is not Wolf) return 0;
        Wolf other = (Wolf)obj;
        return (int) (BearingToPrey - other.BearingToPrey);
    }

    private double InvertBearing(double bearing)
    {
        if (bearing is > 360 or < 0) Console.WriteLine("Somethings not right with the bearing");
        if(bearing > 180 ) return bearing - 180;
        if (bearing <= 180) return bearing + 180;
        return -1;
    }
}