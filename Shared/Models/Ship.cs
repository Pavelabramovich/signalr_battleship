namespace BattleShip.Shared.Models;



public class Ship
{
    /// <include file='Documentation/Ship.xml' path='doc/class[@name="Ship"]/method[@name="X"]' />
    public int X { get; init; }
    /// <include file='Documentation/Ship.xml' path='doc/class[@name="Ship"]/method[@name="Y"]' />
    public int Y { get; init; }

    /// <include file='Documentation/Ship.xml' path='doc/class[@name="Ship"]/method[@name="Size"]' />
    public int Size { get; init; }
    /// <include file='Documentation/Ship.xml' path='doc/class[@name="Ship"]/method[@name="Orientation"]' />
    public Orientation Orientation { get; init; }


    /// <include file='Documentation/Ship.xml' path='doc/class[@name="Ship"]/method[@name="Ctor"]' />
    public Ship(int x, int y, int size, Orientation orientation)
    {
        if (x < 0)
            throw new ArgumentException("X must be not negative.", nameof(x));

        if (y < 0)
            throw new ArgumentException("Y must be not negative.", nameof(y));

        if (size <= 0)
            throw new ArgumentException("Size must be positive.", nameof(size));

        X = x;
        Y = y;

        Size = size;
        Orientation = orientation;
    }

    /// <include file='Documentation/Ship.xml' path='doc/class[@name="Ship"]/method[@name="Deconstruct"]' />
    public void Deconstruct(out int x, out int y, out int size, out Orientation orientation)
    {
        x = X;
        y = Y;

        size = Size;
        orientation = Orientation;
    }
}