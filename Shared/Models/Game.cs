using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BattleShip.Shared.Models;


/// <include file='Documentation/BattleShipGame.xml' path='doc/class[@name="BattleShipGame"]/description' />
public class Game<TPlayer>
{
    public TPlayer Player1 { get; init; }
    public TPlayer Player2 { get; init; }

    private readonly Grid _player1Grid;
    private readonly Grid _player2Grid;

    public bool IsPlayer1Turn { get; private set; }


    private Dictionary<Action<TPlayer>, (Action onPlayer1Lost, Action onPlayer2Lost)>? _eventsDictionary;

    /// <include file='Documentation/BattleShipGame.xml' path='doc/class[@name="BattleShipGame"]/method[@name="OnGameEnd"]' />
    public event Action<TPlayer>? OnGameEnd
    {
        add
        {
            if (value is not null)
            {
                Action onPlayer1Lost = () => value(Player2);
                Action onPlayer2Lost = () => value(Player1);

                _eventsDictionary ??= new();

                _eventsDictionary[value] = (onPlayer1Lost, onPlayer2Lost);

                _player1Grid.OnAllShipDestroyed += onPlayer1Lost;
                _player2Grid.OnAllShipDestroyed += onPlayer2Lost;
            }       
        }
        remove
        {
            if (value is not null && _eventsDictionary is not null)
            {
                _player1Grid.OnAllShipDestroyed -= _eventsDictionary[value].onPlayer1Lost;
                _player2Grid.OnAllShipDestroyed -= _eventsDictionary[value].onPlayer2Lost;

                _eventsDictionary.Remove(value);
            }
        }
    }


    internal Game(TPlayer player1, TPlayer player2, Grid player1Grid, Grid player2Grid, bool isPlayer1Move = true)
    {
        Player1 = player1;
        Player2 = player2;

        _player1Grid = player1Grid;
        _player2Grid = player2Grid;

        IsPlayer1Turn = isPlayer1Move;
    }

    public ShotStatus? Player1Turn(int x, int y)
    {
        if (IsPlayer1Turn)
        {
            var res = _player2Grid.Shot(x, y);

            if (res == ShotStatus.Miss)
                IsPlayer1Turn = false;

            return res;
        }

        return null;
    }

    public ShotStatus? Player2Turn(int x, int y)
    {
        if (!IsPlayer1Turn)
        {
            var res = _player1Grid.Shot(x, y);

            if (res == ShotStatus.Miss)
                IsPlayer1Turn = true;

            return res;
        }

        return null;
    }


    public IEnumerable<IEnumerable<BattleSquare>> Player1Rows => _player1Grid.Rows;
    public IEnumerable<IEnumerable<BattleSquare>> Player2Rows => _player2Grid.Rows;

    public BattleSquare[,] Player1Field => _player1Grid.Field;
	public BattleSquare[,] Player2Field => _player2Grid.Field;

	public override string ToString()
    {
        return $"""
            {Player1} grid:
            {_player1Grid}

            {Player2} grid:
            {_player2Grid}

            Turn: {(IsPlayer1Turn ? Player1 : Player2)}
            """;
    }
}
