
namespace BattleShip.Shared.Models;


/// <include file='Documentation/Enums.xml' path='doc/class[@name="Orientation"]/description' />
public enum Orientation
{
    LeftRight = 0,
    TopDown = 1,
}

/// <include file='Documentation/Enums.xml' path='doc/class[@name="Direction"]/description' />
public enum Direction
{
    Top = 0,
    Right = 1,
    Bottom = 2,
    Left = 3,
}

/// <include file='Documentation/Enums.xml' path='doc/class[@name="ShipPart"]/description' />
[Flags]
public enum ShipPart
{
    Center = 0b_00,
    Start =  0b_01,
    End =    0b_10,
}

/// <include file='Documentation/Enums.xml' path='doc/class[@name="OrientedShipPart"]/description' />
public record struct OrientedShipPart(Orientation Orientation, ShipPart ShipPart);
