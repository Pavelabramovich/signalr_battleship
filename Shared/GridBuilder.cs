
namespace BattleShip.Shared;


public class GridBuilder
{
    private readonly BuilderSquare[,] _field;

    private Ship? _currentShip;
    private bool _is_current_ship_valid = true;


    private const int DEFAULT_SIZE = 10;


    public GridBuilder(int sizeX, int sizeY)
    {
        if (sizeX <= 0)
            throw new ArgumentException("Size must be positive", nameof(sizeX));

        if (sizeY <= 0)
            throw new ArgumentException("Size must be positive", nameof(sizeY));

        _field = new BuilderSquare[sizeX, sizeY];
    }

    public GridBuilder()
        : this(DEFAULT_SIZE, DEFAULT_SIZE)
    { }


    public int SizeX => _field.GetLength(0);
    public int SizeY => _field.GetLength(1);


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

        SaveShip();
        _currentShip = ship;


        MarkShip(ship);

        return this;
    }


    public Ship? GetShip(int x, int y)
    {
        // Not implemented.

        return null;
    }


    public GridBuilder SaveShip()
    {
        if (_currentShip is not null)
        {
            if (_is_current_ship_valid)
            {
                MarkBorder(_currentShip, s => s.RemoveGreenBorder());
                MarkBorder(_currentShip, s => s.AddBorderedShip());
            }
            else
            {
                ResetShip(_currentShip, s =>
                {
                    if (s.IsInvalidShip)
                        s = s.RemoveShip().RemoveInvalidShip();

                    return s;
                });


                MarkBorder(_currentShip, s => s.RemoveRedBorder());
            }

            _currentShip = null;
        }

        return this;
    }


    public GridBuilder RemoveShip(Ship ship)
    {
        SaveShip();

        ResetShip(ship, s => s.RemoveShip());
        MarkBorder(ship, s => s.RemoveBorderedShip().RemoveGreenBorder().RemoveRedBorder());

        return this;
    }




    private void ResetShip(Ship ship, Func<BuilderSquare, BuilderSquare> modify)
    {
        ResetShip(ship, (s, i) => modify(s));
    }

    private void ResetShip(Ship ship, Func<BuilderSquare, int, BuilderSquare> modifyByIndex)
    {
        int x = ship.X;
        int y = ship.Y;

        Action moveNext = () => _ = ship.Orientation == Orientation.LeftRight ? x++ : y++;

        for (int i = 0; i < ship.Size; i++, moveNext())
        {
            _field[x, y] = modifyByIndex(_field[x, y], i);
        }
    }


    // cringe
    public void MoveShip(int d)
    {
        var c = new Ship(_currentShip.X, _currentShip.Y, _currentShip.Size, _currentShip.Orientation);

        var (x, y) = (_currentShip.X, _currentShip.Y);

        if (d == 0)
            y--;
        else if (d == 1)
            x++;
        else if (d == 2)
            y++;
        else
            x--;

        var c2 = new Ship(x, y, _currentShip.Size, _currentShip.Orientation);

        SaveShip();

        RemoveShip(c);

        AddShip(c2);
    }



    public Grid Build()
    {
        return new Grid(_field);
    }


    private void MarkShip(Ship ship)
    {
        bool isValid = true;

        ResetShip(ship, s =>
        {
            if (s.IsShip || s.IsBordered)
                isValid = false;

            return s;
        });

        ResetShip(ship, (s, i) =>
        {
            if (!s.IsShip)
            {
                if (!isValid)
                    s = s.AddInvalidShip();

                s = s.AddShip();

                s = (ship.Orientation == Orientation.LeftRight) ? s.SetLeftRight() : s.SetTopDown();


                if (ship.Size == 1)
                {
                    s = s.SetBegin().SetEnd();
                }
                else
                {
                    if (i == 0)
                    {
                        s = s.SetBegin().SetNotEnd();
                    }
                    else if (i == ship.Size - 1)
                    {
                        s = s.SetEnd().SetNotBegin();
                    }
                    else
                    {
                        s = s.SetNotBegin().SetNotEnd();
                    }
                }
            }

            return s;
        });

        MarkBorder(ship, s => isValid ? s.AddGreenBorder() : s.AddRedBorder());

        _is_current_ship_valid = isValid;
    }

    private void MarkBorder(Ship ship, Func<BuilderSquare, BuilderSquare> modify)
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

            _field[x, y] = modify(_field[x, y]);
        }
    }


    public override string ToString()
    {
        return Build().ToString();
    }

}

