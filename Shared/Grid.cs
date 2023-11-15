
namespace BattleShip.Shared;

public class Grid
{
    // Will be replaced by a Battle square
    private readonly BuilderSquare[,] _field;


    internal Grid(BuilderSquare[,] field)
    {
        _field = field;
    }

    public override string ToString()
    {
        string res = string.Empty;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                res += _field[j, i].ToString() + " ";
            }

            res += '\n';
        }

        return res;
    }
}