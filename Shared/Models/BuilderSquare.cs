using System.Drawing;

namespace BattleShip.Shared.Models;


/// <include file='Documentation/BuilderSquare.xml' path='doc/class[@name="BuilderSquare"]/description' />
public record struct BuilderSquare(Color SeaColor, OrientedShipPart? OrientedShipPart)
{
    /// <include file='Documentation/BuilderSquare.xml' path='doc/class[@name="BuilderSquare"]/method[@name="ToString"]' />
    public override readonly string ToString()
    {
        //return $"{SeaColor.Name.ToLower()[0]}_{(OrientedShipPart is null ? "." : OrientedShipPart?.ShipPart.ToString().Substring(0, 3))}";

        if (OrientedShipPart is null)
        {
            return " ";
        }
        else
        {
            return "B";
        }
    }
}