using Microsoft.AspNetCore.SignalR.Client;
using System.Linq.Expressions;

namespace RapidIntegrationTesting.SignalR;

/// <summary>
///     Factory to create <see cref="ISignalRCallbackDescriptor" />.
/// </summary>
public static class SignalRCallbackDescriptorFactory
{
    /// <summary>
    ///     Produces a Descriptor meant to be added a <see cref="HubConnection" />
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="handler">The handler to be added to the <see cref="HubConnection" /></param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface>(Expression<Func<TInterface, Task>> expression, Action handler)
    {
        ArgumentNullException.ThrowIfNull(expression);

        string name = expression.GetNameAndEnsureParameters(0);
        return new SignalRCallbackDescriptorNoValue(name, handler);
    }

    /// <summary>
    ///     Produces a Descriptor meant to be added a <see cref="HubConnection" />
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="handler">The handler to be added to the <see cref="HubConnection" /></param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface>(Expression<Func<TInterface, Task>> expression, Func<Task> handler)
    {
        ArgumentNullException.ThrowIfNull(expression);

        string name = expression.GetNameAndEnsureParameters(0);
        return new SignalRAsyncCallbackDescriptorNoValue(name, handler);
    }

    /// <summary>
    ///     Produces a Descriptor meant to be added a <see cref="HubConnection" />
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <typeparam name="TArgument">The argument to the method</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="handler">The handler to be added to the <see cref="HubConnection" /></param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface, TArgument>(Expression<Func<TInterface, TArgument, Task>> expression, Action<TArgument> handler)
    {
        ArgumentNullException.ThrowIfNull(expression);

        string name = expression.GetNameAndEnsureParameters(1, typeof(TArgument));
        return new SignalRCallbackDescriptor<TArgument>(name, handler);
    }

    /// <summary>
    ///     Produces a Descriptor meant to be added a <see cref="HubConnection" />
    /// </summary>
    /// <typeparam name="TInterface">The interface for the Hub</typeparam>
    /// <typeparam name="TArgument">The argument to the method</typeparam>
    /// <param name="expression">Expression detailing for which hub method a descriptor is to be returned</param>
    /// <param name="handler">The handler to be added to the <see cref="HubConnection" /></param>
    /// <returns>The created Descriptor</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ISignalRCallbackDescriptor Create<TInterface, TArgument>(Expression<Func<TInterface, TArgument, Task>> expression, Func<TArgument, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(expression);

        string name = expression.GetNameAndEnsureParameters(1, typeof(TArgument));
        return new SignalRAsyncCallbackDescriptor<TArgument>(name, handler);
    }
}