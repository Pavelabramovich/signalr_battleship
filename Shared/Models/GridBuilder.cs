using System.Drawing;

namespace BattleShip.Shared.Models;


public class GridBuilder
{
    private Ship? _selectedShip;
    private Dictionary<Point, Ship> _ships;

    private const int DEFAULT_SIZE = 10;


    public GridBuilder(int sizeX, int sizeY)
    {
        if (sizeX <= 0)
            throw new ArgumentException("Size must be positive", nameof(sizeX));

        if (sizeY <= 0)
            throw new ArgumentException("Size must be positive", nameof(sizeY));

        SizeX = sizeX;
        SizeY = sizeY;

        _ships = new Dictionary<Point, Ship>();
    }

    public GridBuilder()
        : this(DEFAULT_SIZE, DEFAULT_SIZE)
    { }


    public int SizeX { get; init; }
    public int SizeY { get; init; }


    public GridBuilder AddShip(Ship ship)
    {
        if (ship.X >= SizeX)
            throw new ArgumentOutOfRangeException(nameof(ship.X), "Invalid ship x.");

        if (ship.Y >= SizeY)
            throw new ArgumentOutOfRangeException(nameof(ship.Y), "Invalid ship y.");

        if (ship.Orientation == Orientation.LeftRight && ship.X + ship.Size > SizeX
            || ship.Orientation == Orientation.TopDown && ship.Y + ship.Size > SizeY)
        {
            throw new ArgumentOutOfRangeException(nameof(ship.Size), "Invalid ship size.");
        }

        SaveSelectedShip();
        _selectedShip = ship;

        return this;
    }

    private GridBuilder SaveSelectedShip()
    {
        if (_selectedShip is null)
            return this;

        if (IsSelectedShipValid())
        {
            _ships[new Point(_selectedShip.X, _selectedShip.Y)] = _selectedShip;
        }

        _selectedShip = null;

        return this;
    }

    public GridBuilder RemoveSelectedShip()
    {
        _selectedShip = null;

        return this;
    }


    public GridBuilder SelectShip(int x, int y)
    {
        foreach (Ship ship in _ships.Values)
        {
            if (IsIntersectedWith(x, y, ship))
            {
                _selectedShip = ship;
                _ships.Remove(new Point(ship.X, ship.Y));

                return this;
            }
        }

        return this;
    }


    private static bool IsIntersectedWith(int x, int y, Ship ship)
    {
        if (ship.Orientation == Orientation.LeftRight)
        {
            return y == ship.Y && x >= ship.X && x < (ship.X + ship.Size);
        }
        else
        {
            return x == ship.X && y >= ship.Y && y < (ship.Y + ship.Size);
        }
    }

    private static void MoveNext(ref int x, ref int y, Ship ship) => _ = ship.Orientation == Orientation.LeftRight ? x++ : y++;

    private bool IsSelectedShipValid()
    {
        if (_selectedShip is null)
            return true;

        foreach (Ship ship in _ships.Values)
        {
            int x = ship.X;
            int y = ship.Y;

            for (int i = 0; i < ship.Size; i++, MoveNext(ref x, ref y, ship))
            {
                if (IsIntersectedWith(x, y, _selectedShip))
                {
                    return false;
                }
            }
        }

        return true;
    }


    private static void ResetShip(Ship ship, Func<Color, Color> modify, Color[,] colors)
    {
        ResetShip(ship, (s, i) => modify(s), colors);
    }

    private static void ResetShip(Ship ship, Func<Color, int, Color> modifyByIndex, Color[,] colors)
    {
        int x = ship.X;
        int y = ship.Y;

        Action moveNext = () => _ = ship.Orientation == Orientation.LeftRight ? x++ : y++;

        for (int i = 0; i < ship.Size; i++, moveNext())
        {
            colors[x, y] = modifyByIndex(colors[x, y], i);
        }
    }


    public GridBuilder MoveSelectedShip(Direction direction)
    {
        if (_selectedShip is null)
            return this;

        var ship = _selectedShip;

        RemoveSelectedShip();

        (int x, int y) = direction switch
        {
            Direction.Top => (ship.X, ship.Y - 1),
            Direction.Right => (ship.X + 1, ship.Y),
            Direction.Bottom => (ship.X, ship.Y + 1),
            Direction.Left => (ship.X - 1, ship.Y),
            _ => throw new NotImplementedException("Unknown direction type")
        };

        AddShip(new Ship(x, y, ship.Size, ship.Orientation));

        return this;
    }


    private void MarkBorder(Ship ship, Func<Color, Color> modify, Color[,] colors)
    {
        var (x, y, size, Orientation) = ship;

        for (int i = -1; i < size + 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j != 0 || i < 0 || i >= size)
                {
                    if (Orientation == Orientation.LeftRight)
                    {
                        MarkSquare(x + i, y + j);
                    }
                    else
                    {
                        MarkSquare(x + j, y + i);
                    }
                }
            }
        }

        void MarkSquare(int x, int y)
        {
            if (x < 0 || x >= SizeX || y < 0 || y >= SizeY)
                return;

            colors[x, y] = modify(colors[x, y]);
        }
    }


    public override string ToString()
    {
        return Build().ToString();
    }

    public Grid Build()
    {
        var colors = new Color[SizeX, SizeY];
        

        foreach (var ship in _ships.Values)
        {
            ResetShip(ship, c => Color.Black, colors);
        }

        if (_selectedShip is not null)
        {
            Color borderColor = IsSelectedShipValid() ? Color.Green : Color.Red;

            ResetShip(_selectedShip, c => Color.Gray, colors);

            int x = _selectedShip.X;
            int y = _selectedShip.Y;

            MarkBorder(_selectedShip, c => borderColor, colors);
        }

        return new Grid(colors);
    }

}

