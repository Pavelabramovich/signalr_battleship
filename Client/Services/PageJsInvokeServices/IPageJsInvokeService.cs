namespace BattleShip.Client.Services;


public interface IPageJsInvokeService
{
	public Task RegisterAsync<T>(T page) where T : class;

	public Task UnregisterAsync<T>() where T : class;
}
