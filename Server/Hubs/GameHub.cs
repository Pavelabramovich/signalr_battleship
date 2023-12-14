
using Microsoft.AspNetCore.SignalR;

namespace BattleShip.Server.Hubs;

public class GameHub : Hub
{
    private static readonly Dictionary<string, List<Tuple<string, string?>>> GameGroups = 
		new Dictionary<string, List<Tuple<string, string?>>>();
    public async Task CreateGame(string game, string username)
    {
        if (GameGroups.ContainsKey(game))
        {
            throw new Exception("This game already exists!");
        }

        GameGroups.Add(game, new List<Tuple<string, string?>>());

        try
        {
			await Groups.AddToGroupAsync(Context.ConnectionId, game);
		}
        catch(Exception ex) 
        {
            Console.WriteLine(ex.Message);
        }

        await JoinGame(game, username);
    }

    public async Task JoinGame(string game, string username)
    {
		if (!GameGroups.ContainsKey(game))
		{
			throw new Exception("This game doesn't exists!");
		}

		if (GameGroups[game].Count > 1) 
        {
            throw new Exception("This game is full!");
        }

        GameGroups[game].Add(new Tuple<string, string?>(username, null));

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
		int index = GameGroups[game].FindIndex(tuple => tuple.Item1 == username);

		if (index != -1)
		{
			GameGroups[game][index] = new Tuple<string, string?>(username, field);
		}

		if (GameGroups[game][0].Item2 == null || GameGroups[game][1].Item1 == null)
		{
			await Clients.Group(game).SendAsync("Wait");
		}
		else
		{
			await Clients.Group(game).SendAsync("Start");
		}	
	}
}