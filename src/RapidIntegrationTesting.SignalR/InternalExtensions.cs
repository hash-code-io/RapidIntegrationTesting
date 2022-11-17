using System.Linq.Expressions;
using System.Reflection;

namespace RapidIntegrationTesting.SignalR;

internal static class InternalExtensions
{
    internal static async Task TimeoutAfter(this Task task, int millisecondsTimeout)
    {
        if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)).ConfigureAwait(false))
            await task.ConfigureAwait(false);
        else
            throw new TimeoutException();
    }

    internal static async Task<T> TimeoutAfter<T>(this Task<T> task, int millisecondsTimeout)
    {
        if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)).ConfigureAwait(false))
            return await task.ConfigureAwait(false);

        throw new TimeoutException();
    }

    internal static ParameterInfo[] EnsureParameters(this MethodInfo methodInfo, int numberExpectedParams, params Type[] expectedTypes)
    {
        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        if (parameterInfos.Length != numberExpectedParams)
            throw new InvalidOperationException($"Method didn't have exactly {numberExpectedParams} parameter");

        for (int i = 0; i < numberExpectedParams; i++)
        {
            Type expected = expectedTypes[i];
            Type actual = parameterInfos[i].ParameterType;
            if(expected != actual)
                throw new InvalidOperationException($"Given method argument was of type {actual.Name}, but {expected.Name} was expected");
        }

        return parameterInfos;
    }

    internal static string GetNameAndEnsureParameters(this LambdaExpression lambdaExpression, int numberExpectedParams, params Type[] expectedTypes)
    {
        if (lambdaExpression.Body.GetType().IsAssignableFrom(typeof(MethodCallExpression)))
            throw new InvalidOperationException("Given expression has to be or derive from a MethodCallExpression");

        var body = (MethodCallExpression)lambdaExpression.Body;
        MethodInfo methodInfo = body.Method;

        string name = methodInfo.Name;
        _ = methodInfo.EnsureParameters(numberExpectedParams, expectedTypes);
        return name;
    }
}