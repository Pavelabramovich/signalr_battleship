using Microsoft.AspNetCore.SignalR.Client;

namespace BattleShip.Client.Services
{
    public interface IGameService
	{
        public Task ConnectToHub();
        public void CreateConnection(string method, Action handler);
        public Task<bool> CreateGame(string game, string username);
        public Task<bool> JoinGame(string game, string username);
    }
}
