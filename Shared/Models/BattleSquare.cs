
using System.Drawing;

namespace BattleShip.Shared.Models;


/// <include file='Documentation/BattleSquare.xml' path='doc/class[@name="BattleSquare"]/description' />
public class BattleSquare
{
    public ShotStatus ShotStatus { get; internal set; }
    public OrientedShipPart? OrientedShipPart { get; private set; }


    public BattleSquare(ShotStatus shotStatus, OrientedShipPart? orientedShipPart)
    {
        ShotStatus = shotStatus;
        OrientedShipPart = orientedShipPart;
    }

    public void Deconstruct(out ShotStatus shotStatus, out OrientedShipPart? orientedShipPart)
    {
        shotStatus = ShotStatus;
        orientedShipPart = OrientedShipPart;
    }

    /// <include file='Documentation/BattleSquare.xml' path='doc/class[@name="BattleSquare"]/method[@name="ToString"]' />
    public override string ToString()
    {
        return this switch
        {
            (ShotStatus.Intact, orientedShipPart: null) => " ",
            (ShotStatus.Intact, orientedShipPart: not null) => "█",
            (ShotStatus.Shotted, orientedShipPart: null) => "·",
            (ShotStatus.Shotted, orientedShipPart: not null) => "X",
            (ShotStatus.Destroyed, _) => "☼",
            _ => throw new NotImplementedException("Uncnown pattern."),
        };
    }
}
