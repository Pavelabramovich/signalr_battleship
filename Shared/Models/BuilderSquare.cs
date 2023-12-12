using System.Drawing;

namespace BattleShip.Shared.Models;


/// <include file='Documentation/BuilderSquare.xml' path='doc/class[@name="BuilderSquare"]/description' />
public record struct BuilderSquare(Color SeaColor, OrientedShipPart? OrientedShipPart)
{
    public override readonly string ToString()
    {
        return $"{SeaColor.Name.ToLower()[0]}_{(OrientedShipPart is null ? "." : OrientedShipPart?.ShipPart.ToString().Substring(0, 3))}";
    }
}