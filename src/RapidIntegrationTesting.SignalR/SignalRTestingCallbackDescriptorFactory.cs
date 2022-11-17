using Microsoft.AspNetCore.SignalR.Client;
using System.Linq.Expressions;

namespace RapidIntegrationTesting.SignalR;

/// <summary>
///     Factory to create <see cref="ISignalRCallbackDescriptor" />. Methods contained are meant for testing purposes only
/// </summary>
public static class SignalRTestingCallbackDescriptorFactory
{
    /// <summary>
    ///     Meant for testing purposes only. Produces a Descriptor meant to be added a <see cref="HubConnection" />. As soon as
    ///     an invocation
    ///     is received the <paramref name="signalReceived" /> task triggers
    ///     Any additional invocations will throw an exception.
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <typeparam name="TArgument">The argument to the method</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="signalReceived">Task that finishes as soon as an invocation is received</param>
    /// <param name="timeout">Amount to timeout after in milliseconds</param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface, TArgument>(Expression<Func<TInterface, TArgument, Task>> expression, out Task<TArgument> signalReceived, int timeout = 30_000)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        string name = expression.GetNameAndEnsureParameters(1, typeof(TArgument));
        var tcs = new TaskCompletionSource<TArgument>();
        signalReceived = tcs.Task.TimeoutAfter(timeout);

        return new SignalRCallbackDescriptor<TArgument>(name, arg => tcs.SetResult(arg));
    }

    /// <summary>
    ///     Meant for testing purposes only. Produces a Descriptor meant to be added a <see cref="HubConnection" />. As soon as
    ///     an invocation
    ///     is received the <paramref name="signalReceived" /> task triggers
    ///     Any additional invocations will throw an exception.
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="signalReceived">Task that finishes as soon as an invocation is received</param>
    /// <param name="timeout">Amount to timeout after in milliseconds</param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface>(Expression<Func<TInterface, Task>> expression, out Task signalReceived, int timeout = 30_000)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        string name = expression.GetNameAndEnsureParameters(0);
        var tcs = new TaskCompletionSource();
        signalReceived = tcs.Task.TimeoutAfter(timeout);

        return new SignalRCallbackDescriptorNoValue(name, () => tcs.SetResult());
    }
}