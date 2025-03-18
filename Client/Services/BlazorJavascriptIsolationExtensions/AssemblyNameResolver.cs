using System.Reflection;
using Microsoft.AspNetCore.Components;


namespace BlazorJavascriptIsolationExtensions;


public interface IAssemblyNameResolver
{
	string GetEntryAssembly();

	string GetComponentAssembly<TComponent>()
		where TComponent : ComponentBase;

	bool IsExternalAssembly<TComponent>()
		where TComponent : ComponentBase;
}

public class AssemblyNameResolver : IAssemblyNameResolver
{
	public virtual string GetEntryAssembly() => Assembly.GetEntryAssembly().FullName!;

	public virtual string GetComponentAssembly<TComponent>()
		where TComponent : ComponentBase => typeof(TComponent).Assembly.FullName!;

	public bool IsExternalAssembly<TComponent>()
		where TComponent : ComponentBase => GetEntryAssembly() != GetComponentAssembly<TComponent>();
}