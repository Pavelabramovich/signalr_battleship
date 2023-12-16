using Microsoft.AspNetCore.SignalR;
using System;
using System.Data.Common;

namespace BattleShip.Server.Hubs;

public class GameHub : Hub
{
    private static readonly Dictionary<string, List<Tuple<string, string, string?>>> GameGroups = 
		new Dictionary<string, List<Tuple<string, string, string?>>>();

	private static Dictionary<string, string> FieldContent = new Dictionary<string, string>();
	private static Dictionary<string, string> GameMove = new Dictionary<string, string>();
	public async Task CreateGame(string game, string username)
    {
        if (GameGroups.ContainsKey(game))
        {
            throw new HubException("This game already exists!");
        }

        GameGroups.Add(game, new List<Tuple<string, string, string?>>());

        await JoinGame(game, username);
    }

    public async Task JoinGame(string game, string username)
    {
		if (!GameGroups.ContainsKey(game))
		{
			throw new HubException("This game doesn't exists!");
		}

		if (GameGroups[game].Count > 1) 
        {
            throw new HubException("This game is full!");
        }

		if (GameGroups[game].FindIndex(tuple => tuple.Item2 == username) != -1)
		{
			throw new HubException("User with this name already exists");
		}

        GameGroups[game].Add(new Tuple<string, string, string?>(Context.ConnectionId, username, null));

		try
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, game);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

		try
		{
			await Clients.Group(game).SendAsync("Receive");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

        if (GameGroups[game].Count == 2)
        {
			await Clients.Group(game).SendAsync("Notify");
		}
    }

    public async Task StartGame(string game, string username, string field)
    {
		await Console.Out.WriteLineAsync(game);

		await Console.Out.WriteLineAsync(field + "<---");

		int index = GameGroups[game].FindIndex(tuple => tuple.Item2 == username);

		if (index != -1)
		{
			await Console.Out.WriteLineAsync("NOT -1");
			GameGroups[game][index] = new Tuple<string, string, string?>(Context.ConnectionId, username, field);
		}

		try
		{
			if (GameGroups[game][0].Item3 == null || GameGroups[game][1].Item3 == null)
			{
				await Clients.Client(Context.ConnectionId).SendAsync("Wait");
			}
			else
			{
				await Clients.Group(game).SendAsync("Start");
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			await Console.Out.WriteLineAsync($"{e.Message}");
		}
	}

	public string GetOpponentField(string game, string username)
	{
		int index = GameGroups[game].FindIndex(tuple => tuple.Item2 != username);

		Console.WriteLine(GameGroups.Keys.Aggregate((k1, k2) => $"{k1}, {k2}") + " GameGroup keys in GameHub");

		if (index != -1)
		{
			Console.WriteLine(GameGroups[game][index].Item3);
			return GameGroups[game][index].Item3!;
		}
		else
		{
			return "";
		}
	}

	public async Task Move(string game, string username, int x, int y, bool shot)
	{
		int index = GameGroups[game].FindIndex(tuple => tuple.Item2 != username);

		if (index != -1)
		{
			await Clients.Client(GameGroups[game][index].Item1).SendAsync("GetMove", x, y, shot);
		}

		if (!shot)
		{
			await Clients.Group(game).SendAsync("Change");
		}
	}
	public string GetContent(string username)
	{
		return FieldContent[username];
	}
	public void AddContent(string username, string content)
	{
		//Console.WriteLine(content + "<++++++++++");
		FieldContent.Add(username, content);
	}
	public string GetMove(string game)
	{
		return GameMove[game];
	}
	public void AddMove(string game, string username)
	{
		Console.WriteLine(game + " " + username + " add move");
		if (!GameMove.ContainsKey(game))
		{
			GameMove.Add(game, username);
		}
		else
		{
			GameMove[game] = username;
		}
	}

	public async Task EndGame(string game, string username)
	{
		await Clients.Group(game).SendAsync("Finish", username);
	}

	public async Task DeleteGame(string game)
	{
		await Clients.Group(game).SendAsync("End");

		Console.WriteLine("DELETE");

		FieldContent.Remove(GameGroups[game][0].Item2);
		FieldContent.Remove(GameGroups[game][1].Item2);

		await Groups.RemoveFromGroupAsync(GameGroups[game][0].Item1, game);
		await Groups.RemoveFromGroupAsync(GameGroups[game][1].Item1, game);

		

		GameMove.Remove(game);
		GameGroups.Remove(game);
	}
}