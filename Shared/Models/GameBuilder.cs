using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;


namespace BattleShip.Shared.Models;


public class GameBuilder<TPlayer> where TPlayer : notnull
{
    private readonly Dictionary<TPlayer, Grid?> _playerGrids;


    public GameBuilder()
    {
        _playerGrids = new Dictionary<TPlayer, Grid?>();
    }


    public IEnumerable<(TPlayer, Grid?)> PlayerGrids
    {
        get => _playerGrids.Select(kv => (kv.Key, kv.Value));
    }


    public GameBuilder<TPlayer> AddPlayer(TPlayer player)
    {
        ArgumentNullException
            .ThrowIfNull(player, nameof(player));

        if (_playerGrids.Count >= 2)
            throw new InvalidOperationException("Two players are already in the game.");

        if (_playerGrids.ContainsKey(player))
            throw new InvalidOperationException("This player is already in the game.");


        _playerGrids[player] = null;

        return this;
    }

    public GameBuilder<TPlayer> AddPlayerGrid(TPlayer player, Grid playerGrid)
    {
        ArgumentNullException
            .ThrowIfNull(player, nameof(player));

        ArgumentNullException
           .ThrowIfNull(playerGrid, nameof(playerGrid));

        if (!_playerGrids.ContainsKey(player))
            throw new InvalidOperationException("This player has not been added to the game.");

        if (_playerGrids[player] is not null)
            throw new InvalidOperationException("This player already has a grid.");


        _playerGrids[player] = playerGrid;

        return this;
    }

    public bool IsTwoPlayers => _playerGrids.Count == 2;
    public bool IsReadyToPlay
    {
        get
        {
            if (_playerGrids.Count != 2)
                return false;

            foreach (var playerGrid in _playerGrids)
            {
                if (playerGrid.Value is null)
                    return false;
            }

            return true;
        }
    }


    public Game<TPlayer> Build()
    {
        if (_playerGrids.Count != 2)
            throw new InvalidOperationException("Exactly two players are required to start the game.");

        foreach (var playerGrid in _playerGrids)
        {
            if (playerGrid.Value is null)
                throw new InvalidOperationException($"{playerGrid.Key} has no grid.");
        }

        var (player1, player1Grid) = _playerGrids.First();
        var (player2, player2Grid) = _playerGrids.Last();

        return new Game<TPlayer>(player1, player2, player1Grid!, player2Grid!);
    }
}
