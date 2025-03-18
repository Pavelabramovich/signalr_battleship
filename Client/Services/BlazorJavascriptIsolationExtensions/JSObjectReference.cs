using System.Dynamic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;


namespace BlazorJavascriptIsolationExtensions;


public interface IJSObjectReference<TComponent> : IJSObjectReference
	where TComponent : ComponentBase
{
	public dynamic Exports { get; }
}

public class JSObjectReference<TComponent> : DynamicObject, IJSObjectReference<TComponent>
	where TComponent : ComponentBase
{
	private readonly IJSRuntime _js;

	private Lazy<Task<IJSObjectReference>> JSModule { get; }

	public dynamic Exports => this;

	public JSObjectReference(IJSRuntime js, IOptions<JavaScriptReferencesOptions> options, IAssemblyNameResolver assemblyNameResolver)
	{
		_js = js;

		JSModule = new Lazy<Task<IJSObjectReference>>(() =>
			_js.ImportComponentScriptAsync<TComponent>(options.Value, assemblyNameResolver));
	}

	public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		var module = await JSModule.Value;
		var result = await module.InvokeAsync<TValue>(identifier, args);
		return result;
	}

	public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		var module = await JSModule.Value;
		return await module.InvokeAsync<TValue>(identifier, cancellationToken, args);
	}

	public async ValueTask DisposeAsync()
	{
		var module = await JSModule.Value;
		await module.DisposeAsync();
	}

	public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
	{
		var csharpBinder = binder.GetType().GetInterface("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");

		if (csharpBinder != null)
		{
			var typeArgs = csharpBinder.GetProperty("TypeArguments")?.GetValue(binder, null) as IList<Type>;

			if (typeArgs?.Count == 0)
			{
				result = InvokeAsync<IJSVoidResult>(binder.Name, args);
				return true;
			}
			else if (typeArgs?.Count == 1)
			{
				var method = typeof(JSObjectReference<TComponent>).GetMethod(nameof(InvokeAsync), new[] { typeof(string), typeof(object?[]) });
				var generic = method?.MakeGenericMethod(typeArgs[0]);

				if (generic != null)
				{
					result = generic.Invoke(this, new object[] { binder.Name, args ?? new object[] { } });
					return true;
				}
			}
		}

		result = null;
		return false;
	}
}
