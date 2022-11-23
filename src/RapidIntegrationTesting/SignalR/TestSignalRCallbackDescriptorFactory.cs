using Microsoft.AspNetCore.SignalR.Client;
using System.Linq.Expressions;

namespace RapidIntegrationTesting.SignalR;

/// <summary>
///     Factory to create <see cref="ISignalRCallbackDescriptor" /> and set up signal receiving
/// </summary>
public static class TestSignalRCallbackDescriptorFactory
{
    /// <summary>
    ///     Produces a Descriptor meant to be added a <see cref="HubConnection" />
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <typeparam name="TArgument">The argument to the method</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="signalReceived">The task to await to receive the signal</param>
    /// <param name="timeout">The time to wait for reception of a signal. Defaults to 30s</param>
    /// <returns>The created Descriptor</returns>
    public static ISignalRCallbackDescriptor Create<TInterface, TArgument>(Expression<Func<TInterface, TArgument, Task>> expression, out Task<TArgument> signalReceived, int timeout = 30_000)
    {
        var tcs = new TaskCompletionSource<TArgument>();
        signalReceived = TimeoutAfter(tcs.Task, timeout);
        return SignalRCallbackDescriptorFactory.Create(expression, arg => tcs.SetResult(arg));
    }

    /// <summary>
    ///     Produces a Descriptor meant to be added a <see cref="HubConnection" />
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="signalReceived">The task to await to receive the signal</param>
    /// <param name="timeout">The time to wait for reception of a signal. Defaults to 30s</param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface>(Expression<Func<TInterface, Task>> expression, out Task signalReceived, int timeout = 30_000)
    {
        var tcs = new TaskCompletionSource();
        signalReceived = TimeoutAfter(tcs.Task, timeout);
        return SignalRCallbackDescriptorFactory.Create(expression, () => tcs.SetResult());
    }

    private static async Task TimeoutAfter(Task task, int millisecondsTimeout)
    {
        if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)).ConfigureAwait(false))
            await task.ConfigureAwait(false);
        else
            throw new TimeoutException();
    }

    private static async Task<T> TimeoutAfter<T>(Task<T> task, int millisecondsTimeout)
    {
        if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)).ConfigureAwait(false))
            return await task.ConfigureAwait(false);

        throw new TimeoutException();
    }
}