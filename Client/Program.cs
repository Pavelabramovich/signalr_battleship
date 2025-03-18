using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BattleShip.Client.Services;
using BlazorJavascriptIsolationExtensions;
using BattleShip.Client;
using Microsoft.JSInterop;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


//builder.Services.AddSingleton<IJSRuntime, JSRuntime>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPageJsInvokeService, PageJsInvokeService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddJavascriptReferences(options =>
{
	options.MapNamespace("BattleShip.Client.Pages",
		(assembly, component, isExternalAssembly) => $"./Pages/{component}.razor.js");
});


await builder.Build().RunAsync();
