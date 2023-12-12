using System.Drawing;

namespace BattleShip.Shared.Models;


/// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/description' />
public class GridBuilder
{
    private Ship? _selectedShip;
    private Dictionary<Point, Ship> _ships;

    private const int DEFAULT_SIZE = 10;

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="Ctor"]/base' />
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

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="Ctor"]/default' />
    public GridBuilder()
        : this(DEFAULT_SIZE, DEFAULT_SIZE)
    { }


    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="SizeX"]' />
    public int SizeX { get; init; }

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="SizeY"]' />
    public int SizeY { get; init; }


    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="AddShip"]' />
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

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="SaveSelectedShip"]' />
    public GridBuilder SaveSelectedShip()
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

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="RemoveSelectedShip"]' />
    public GridBuilder RemoveSelectedShip()
    {
        _selectedShip = null;

        return this;
    }

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="SelectShip"]' />
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

    /// <include file='Documentation/GridBuilder.xml' path='doc/class[@name="GridBuilder"]/method[@name="MoveSelectedShip"]' />
    public GridBuilder MoveSelectedShip(Direction direction)
    {
        if (_selectedShip is null)
            return this;

        var ship = _selectedShip;

        RemoveSelectedShip();

        try
        {
            (int x, int y) = direction switch
            {
                Direction.Top => (ship.X, ship.Y - 1),
                Direction.Right => (ship.X + 1, ship.Y),
                Direction.Bottom => (ship.X, ship.Y + 1),
                Direction.Left => (ship.X - 1, ship.Y),
                _ => throw new NotImplementedException("Unknown direction type")
            };

            AddShip(new Ship(x, y, ship.Size, ship.Orientation));
        }
        catch (ArgumentException)
        {
            AddShip(ship);
        }

        return this;
    }

    public IEnumerable<IEnumerable<BuilderSquare>> Rows
    {
        get
        {
            var colors = new BuilderSquare[SizeX, SizeY];

            Func<Ship, Func<BuilderSquare, int, BuilderSquare>> modifyShipPart = (ship) => (s, i) =>
            {
                ShipPart newShipPart;

                if (ship.Size == 1)
                {
                    newShipPart = ShipPart.Start | ShipPart.End;
                }
                else if (i == 0)
                {
                    newShipPart = ShipPart.End;
                }
                else if (i == ship.Size - 1)
                {
                    newShipPart = ShipPart.Start;
                }
                else
                {
                    newShipPart = ShipPart.Center;
                }

                return s with { OrientedShipPart = (s.OrientedShipPart ?? new()) with { ShipPart = newShipPart } };
            };

            Func<Ship, Func<BuilderSquare, BuilderSquare>> modifyOrientation = (ship) => (s) =>
            {
                return s with { OrientedShipPart = (s.OrientedShipPart ?? new()) with { Orientation = ship.Orientation } };
            };

            foreach (var ship in _ships.Values)
            {
                ResetShip(ship, modifyShipPart(ship), colors);
                ResetShip(ship, modifyOrientation(ship), colors);
            }

            if (_selectedShip is not null)
            {
                Color borderColor = IsSelectedShipValid() ? Color.Green : Color.Red;

                Func<BuilderSquare, BuilderSquare> modifyBorderColor = (s) =>
                {
                    return s with { SeaColor = borderColor };
                };

                ResetShip(_selectedShip, modifyShipPart(_selectedShip), colors);
                ResetShip(_selectedShip, modifyOrientation(_selectedShip), colors);

                MarkBorder(_selectedShip, modifyBorderColor, colors);
            }

            for (int i = 0; i < SizeY; i++)
            {
                yield return Enumerable
                                .Range(0, colors.GetLength(1))
                                .Select(j => colors[i, j]);
            }
        }
    }

    public Grid Build()
    {
        var colors = new Color[SizeX, SizeY];


        //foreach (var ship in _ships.Values)
        //{
        //    ResetShip(ship, c => Color.Black, colors);
        //}

        //if (_selectedShip is not null)
        //{
        //    Color borderColor = IsSelectedShipValid() ? Color.Green : Color.Red;

        //    ResetShip(_selectedShip, c => Color.Gray, colors);

        //    int x = _selectedShip.X;
        //    int y = _selectedShip.Y;

        //    MarkBorder(_selectedShip, c => borderColor, colors);
        //}

        return new Grid(colors);
    }

    public override string ToString()
    {
        string res = string.Empty;

        foreach (var row in this.Rows)
        {
            foreach (var square in row)
            {
                res += square.ToString() + "\t ";
            }

            res += '\n';
        }

        return res;
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


    private static void ResetShip(Ship ship, Func<BuilderSquare, BuilderSquare> modify, BuilderSquare[,] grid)
    {
        ResetShip(ship, (s, i) => modify(s), grid);
    }

    private static void ResetShip(Ship ship, Func<BuilderSquare, int, BuilderSquare> modifyByIndex, BuilderSquare[,] grid)
    {
        int x = ship.X;
        int y = ship.Y;

        for (int i = 0; i < ship.Size; i++, MoveNext(ref x, ref y, ship))
        {
            grid[x, y] = modifyByIndex(grid[x, y], i);
        }
    }


    private void MarkBorder(Ship ship, Func<BuilderSquare, BuilderSquare> modify, BuilderSquare[,] grid)
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

            grid[x, y] = modify(grid[x, y]);
        }
    }




}

