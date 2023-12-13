
using Microsoft.AspNetCore.SignalR;

namespace BattleShip.Server.Hubs;

public class GameHub : Hub
{
    private static readonly Dictionary<string, List<string>> GameGroups = new Dictionary<string, List<string>>();
    public async Task CreateGame(string game, string username)
    {
        if (GameGroups.ContainsKey(game))
        {
            throw new Exception("This game already exists!");
        }

        GameGroups.Add(game, new List<string>());

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

        GameGroups[game].Add(Context.ConnectionId);

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

	public override async Task OnConnectedAsync()
	{
		await Clients.All.SendAsync("Notify");
		await base.OnConnectedAsync();
	}
}