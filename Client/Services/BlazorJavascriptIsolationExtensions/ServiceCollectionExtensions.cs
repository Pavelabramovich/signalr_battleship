using Microsoft.Extensions.DependencyInjection;


namespace BlazorJavascriptIsolationExtensions;


public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddJavascriptReferences(this IServiceCollection services) =>
		AddJavascriptReferences(services, null);

	public static IServiceCollection AddJavascriptReferences(this IServiceCollection services,
		Action<JavaScriptReferencesOptions>? configure)
	{
		services.AddSingleton<IAssemblyNameResolver, AssemblyNameResolver>();

		if (configure != null)
		{
			services.Configure(configure);
		}


		return services.AddTransient(typeof(IJSObjectReference<>), typeof(JSObjectReference<>));
	}
}
