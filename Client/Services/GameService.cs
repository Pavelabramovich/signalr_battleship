using Microsoft.AspNetCore.SignalR.Client;

namespace BattleShip.Client.Services
{
    public class GameService : IGameService
    {
        private readonly HubConnection _connection;

        public GameService()
        {
            _connection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:5001/gamehub")
                        .Build();
		}

        public async Task ConnectToHub()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

		public void CreateConnection(string method, Action handler)
		{
			_connection.On(method, handler);
		}

		public async Task<bool> CreateGame(string game, string username)
        {
            try
            {
				await _connection.InvokeAsync("CreateGame", game, username);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> JoinGame(string game, string username)
        {
            try
            {
				await _connection.InvokeAsync("JoinGame", game, username);
                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.Message);
				return false;
            }
        }
    }
}
