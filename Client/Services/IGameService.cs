namespace BattleShip.Client.Services
{
    public interface IGameService
	{
        public Task ConnectToHub();
        public void CreateConnection(string method, Action handler);
		public void CreateConnection(string method, Action<string> handler);
		public void CreateConnection(string method, Action<int, int, bool> handler);
        public Task<bool> CreateGame(string game, string username);
        public Task<bool> JoinGame(string game, string username);
		public Task<bool> StartGame(string game, string username, string field);
        public Task<string> GetOpponentField(string game, string username);
        public Task Move(string game, string username, int x, int y, bool shot);
		public Task<string> GetContent(string username);
		public Task AddContent(string username, string content);
		public Task<string> GetMove(string game);
		public Task AddMove(string game, string username);
        public Task EndGame(string game, string username);
        public Task DeleteGame(string game);
	}
}
