using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace BlazorJavascriptIsolationExtensions;


public static class JSRuntimeExtensions
{
	public static async Task<IJSObjectReference> ImportComponentScriptAsync<TComponent>(this IJSRuntime js, IPathFormatterResolver formatterResolver, IAssemblyNameResolver assemblyNameResolver)
		where TComponent : ComponentBase
	{
		var pathFormatter = formatterResolver.ResolvePathFormatter<TComponent>();

		if (pathFormatter != null)
		{
			var path = pathFormatter(typeof(TComponent).Assembly.GetName().Name!, typeof(TComponent).Name, assemblyNameResolver.IsExternalAssembly<TComponent>());

            var module = await js.InvokeAsync<IJSObjectReference>("import", path);
			return module;
		}
		else
		{
			throw new InvalidOperationException($"Unable to resolve javascript file path since there is no matching path formatter and the default formatter is not defined.");
		}
	}
}
