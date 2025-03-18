using Microsoft.AspNetCore.Components;


namespace BlazorJavascriptIsolationExtensions;


public interface IPathFormatterResolver
{
	PathFormatter ResolvePathFormatter<TComponent>() where TComponent : ComponentBase;
}


public sealed class JavaScriptReferencesOptions : IPathFormatterResolver
{
	private readonly IDictionary<string, PathFormatter> _assemblyFormatters = new Dictionary<string, PathFormatter>();
	private readonly IDictionary<string, PathFormatter> _namespaceFormatters = new Dictionary<string, PathFormatter>();
	private readonly IDictionary<string, PathFormatter> _componentFormatters = new Dictionary<string, PathFormatter>();

	private PathFormatter _defaultFormatter = (assmebly, component, isExternalAssembly) => !isExternalAssembly 
		? $"./{component}.razor.js" 
		: $"./_content/{assmebly}/{component}.razor.js";

	public JavaScriptReferencesOptions MapAssembly(string assemblyName, PathFormatter pathFormatter)
	{
		_assemblyFormatters[assemblyName] = pathFormatter;
		return this;
	}

	public JavaScriptReferencesOptions MapNamespace(string @namespace, PathFormatter pathFormatter)
	{
		_namespaceFormatters[@namespace] = pathFormatter;
		return this;
	}

	public JavaScriptReferencesOptions MapComponent<TComponent>(PathFormatter pathFormatter)
		where TComponent : ComponentBase
	{
		_componentFormatters[typeof(TComponent).AssemblyQualifiedName!] = pathFormatter;
		return this;
	}

	public JavaScriptReferencesOptions MapDefault(PathFormatter pathFormatter)
	{
		_defaultFormatter = pathFormatter;
		return this;
	}

	public PathFormatter ResolvePathFormatter<TComponent>()
		where TComponent : ComponentBase
	{
		if (!_componentFormatters.TryGetValue(typeof(TComponent).AssemblyQualifiedName!, out var resolver))
		{
			if (!_namespaceFormatters.TryGetValue(typeof(TComponent).Namespace!, out resolver))
			{
				if (!_assemblyFormatters.TryGetValue(typeof(TComponent).Assembly.FullName!, out resolver))
				{
					resolver = _defaultFormatter;
				}
			}
		}

		return resolver;
	}
}
