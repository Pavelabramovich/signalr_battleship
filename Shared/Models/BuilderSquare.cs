using System.Drawing;

namespace BattleShip.Shared.Models;


public record BuilderSquare(Color Color, ShipPart? ShipPart);


// Implementation may change
//public class BuilderSquare
//{
//    //[Flags]
//    //enum Flag : byte
//    //{
//    //    Ship = 0b_1000_0000,


//    //    LeftRight = 0b_0100_0000,
//    //    Begin = 0b_0010_0000,
//    //    End = 0b_0001_0000,



//    //    RedBorder = 0b_0000_0100,
//    //    GreenBorder = 0b_0000_0010,

//    //    InvalidShip = 0b_0000_0001,

//    //    None = 0b_0000_0000,
//    //}


//    //private readonly Flag _flags;

//    private readonly bool _isShip;
//    private readonly Orientation? _orientation;

//    private readonly ShipPart? _shipPart;

//    private readonly bool _isSaved;
//    private readonly Color _color;

//    private readonly int _shipCount;

//    private BuilderSquare(Color color, int shipCount)
//    {

//    }


//    private BuilderSquare(bool isShip, Orientation orientation, ShipPart shipPart, Color color, int shipCount = 0)
//    {
//        _flags = flags;
//        _shipCount = shipCount;
//    }

//    public BuilderSquare()
//        : this(isShip: false)
//    { }


//    public BuilderSquare AddShip()
//    {
//        return AddFlag(Flag.Ship);
//    }
//    public BuilderSquare RemoveShip()
//    {
//        return RemoveFlag(Flag.Ship).SetTopDown().SetNotBegin().SetNotEnd();
//    }


//    public BuilderSquare SetLeftRight() => AddFlag(Flag.LeftRight);
//    public BuilderSquare SetTopDown() => RemoveFlag(Flag.LeftRight);

//    public BuilderSquare SetNotBegin() => RemoveFlag(Flag.Begin);
//    public BuilderSquare SetBegin() => AddFlag(Flag.Begin);

//    public BuilderSquare SetNotEnd() => RemoveFlag(Flag.End);
//    public BuilderSquare SetEnd() => AddFlag(Flag.End);



//    public BuilderSquare AddGreenBorder() => AddFlag(Flag.GreenBorder);
//    public BuilderSquare RemoveGreenBorder() => RemoveFlag(Flag.GreenBorder);

//    public BuilderSquare AddRedBorder() => AddFlag(Flag.RedBorder);
//    public BuilderSquare RemoveRedBorder() => RemoveFlag(Flag.RedBorder);

//    public BuilderSquare AddInvalidShip() => AddFlag(Flag.InvalidShip);
//    public BuilderSquare RemoveInvalidShip() => RemoveFlag(Flag.InvalidShip);



//    public bool IsShip => ContainsFlag(Flag.Ship);


//    public bool IsLeftRight => ContainsFlag(Flag.LeftRight);
//    public bool IsBegin => ContainsFlag(Flag.Begin);
//    public bool IsEnd => ContainsFlag(Flag.End);

//    public bool IsBordered
//    {
//        get => BorderedShipsCount > 0;
//    }
//    public bool IsGreenBorder => ContainsFlag(Flag.GreenBorder);
//    public bool IsRedBorder => ContainsFlag(Flag.RedBorder);

//    public bool IsInvalidShip => ContainsFlag(Flag.InvalidShip);


//    public int BorderedShipsCount => _shipCount;


//    private BuilderSquare AddFlag(Flag flag) => new BuilderSquare(_flags | flag, _shipCount);
//    private BuilderSquare RemoveFlag(Flag flag) => new BuilderSquare(_flags & ~flag, _shipCount);

//    private bool ContainsFlag(Flag flag) => (_flags & flag) == flag;


//    public BuilderSquare AddBorderedShip(int count = 1)
//    {
//        byte bits = (byte)_flags;



//        return new BuilderSquare(_flags, _shipCount + count);
//    }

//    public BuilderSquare RemoveBorderedShip(int count = 1)
//    {
//        return new BuilderSquare(_flags, _shipCount - count);
//    }

//    public BuilderSquare ClearBorderedShips()
//    {
//        return new BuilderSquare(_flags);
//    }



//    public override string ToString()
//    {
//        //  return ((int)_flags).ToString() + " \t";
//        //  return _flags.ToString() + "| ";

//        //return IsShip ? "O" : ".";


//        return this switch
//        {
//            _ when IsShip => "O ",
//            _ when IsRedBorder => "r" + BorderedShipsCount,
//            _ when IsGreenBorder => "g" + BorderedShipsCount,
//            _ when IsBordered => "+" + BorderedShipsCount,
//            _ => ". "
//        };
//    }
//}