using Microsoft.AspNetCore.SignalR.Client;

namespace RapidIntegrationTesting.SignalR;

/// <summary>
///     Descriptor holding information about SignalR Callbacks
/// </summary>
public interface ISignalRCallbackDescriptor
{
    /// <summary>
    ///     Adds the callback of this descriptor to the given <paramref name="hubConnection" />
    /// </summary>
    /// <param name="hubConnection">The connection to add the callback to</param>
    /// <returns>The subscription to the signalR connection. Dispose this to no longer receive notifications of incoming events</returns>
    IDisposable Setup(HubConnection hubConnection);
}

internal abstract class SignalRCallbackDescriptor
{
    protected SignalRCallbackDescriptor(string methodName) => MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));

    protected string MethodName { get; }
}

internal class SignalRCallbackDescriptorNoValue : SignalRCallbackDescriptor, ISignalRCallbackDescriptor
{
    private readonly Action _callback;

    public SignalRCallbackDescriptorNoValue(string methodName, Action callback) : base(methodName) => _callback = callback ?? throw new ArgumentNullException(nameof(callback));

    public IDisposable Setup(HubConnection hubConnection) => hubConnection.On(MethodName, _callback);
}

internal class SignalRAsyncCallbackDescriptorNoValue : SignalRCallbackDescriptor, ISignalRCallbackDescriptor
{
    private readonly Func<Task> _callback;

    public SignalRAsyncCallbackDescriptorNoValue(string methodName, Func<Task> callback) : base(methodName) => _callback = callback ?? throw new ArgumentNullException(nameof(callback));

    public IDisposable Setup(HubConnection hubConnection) => hubConnection.On(MethodName, _callback);
}

internal class SignalRCallbackDescriptor<T> : SignalRCallbackDescriptor, ISignalRCallbackDescriptor
{
    private readonly Action<T> _callback;

    public SignalRCallbackDescriptor(string methodName, Action<T> callback) : base(methodName) => _callback = callback ?? throw new ArgumentNullException(nameof(callback));

    public IDisposable Setup(HubConnection hubConnection) => hubConnection.On(MethodName, _callback);
}

internal class SignalRAsyncCallbackDescriptor<T> : SignalRCallbackDescriptor, ISignalRCallbackDescriptor
{
    private readonly Func<T, Task> _callback;

    public SignalRAsyncCallbackDescriptor(string methodName, Func<T, Task> callback) : base(methodName) => _callback = callback ?? throw new ArgumentNullException(nameof(callback));

    public IDisposable Setup(HubConnection hubConnection) => hubConnection.On(MethodName, _callback);
}