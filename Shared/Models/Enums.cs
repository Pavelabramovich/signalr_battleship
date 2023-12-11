
namespace BattleShip.Shared.Models;


public enum Orientation
{
    LeftRight = 0,
    TopDown = 1,
}

public enum Direction
{
    Top = 0,
    Right = 1,
    Bottom = 2,
    Left = 3,
}

[Flags]
public enum ShipPart
{
    None =   0b_00,
    Start =  0b_01,
    End =    0b_10,
}

