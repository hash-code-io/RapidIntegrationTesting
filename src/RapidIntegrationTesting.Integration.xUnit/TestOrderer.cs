using Xunit.Abstractions;
using Xunit.Sdk;

namespace RapidIntegrationTesting.Integration.xUnit;

/// <summary>
///     Handels the correct ordering of test cases marked with <see cref="TestOrderAttribute" />
///     To register it, use the XUnit attribute: [TestCaseOrderer(nameof(TestOrderer), TestOrderer.AssemblyName)]
/// </summary>
public class TestOrderer : ITestCaseOrderer
{
    /// <summary>
    ///     Type name of the <see cref="TestOrderer" />. Used in the XUnit attribute "TestCaseOrderer"
    /// </summary>
    public const string TypeName = "Testing.Utility.TestOrderer";

    /// <summary>
    ///     Assembly name of the <see cref="TestOrderer" />. Used in the XUnit attribute "TestCaseOrderer"
    /// </summary>
    public const string AssemblyName = "Testing.Utility";

    private readonly IMessageSink _diagnosticMessageSink;

    /// <summary>
    ///     TestOrderer with a given <see cref="IMessageSink" />
    /// </summary>
    /// <param name="diagnosticMessageSink"></param>
    public TestOrderer(IMessageSink diagnosticMessageSink) => _diagnosticMessageSink = diagnosticMessageSink;

    /// <summary>
    ///     Orders tests cases based on the <see cref="TestOrderAttribute" />
    /// </summary>
    /// <typeparam name="TTestCase"></typeparam>
    /// <param name="testCases"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var testCaseList = testCases.ToList();
        var methodWithOrders = testCaseList
                .Select(x => new { Method = x, TestOrders = x.TestMethod.Method.GetCustomAttributes(typeof(TestOrderAttribute)) })
                .Select(x =>
                {
                    if (x.TestOrders.Count() != 1)
                    {
                        string msg =
                            $"TestMethod {x.Method.DisplayName}, was found in an ordered context but did not have an {nameof(TestOrderAttribute)} set";
                        _diagnosticMessageSink.OnMessage(new DiagnosticMessage(msg));
                        throw new InvalidOperationException(msg);
                    }

                    return new { x.Method, TestOrder = x.TestOrders.Single().GetNamedArgument<uint>(nameof(TestOrderAttribute.Order)) };
                })
            ;

        var withOrdersList = methodWithOrders.ToList();
        foreach (var grp in withOrdersList.GroupBy(x => x.TestOrder).Where(grp => grp.Count() > 1))
        {
            string msg = $"Order {grp.Key} was present on multiple TestMethods";
            _diagnosticMessageSink.OnMessage(new DiagnosticMessage(msg));
            throw new InvalidOperationException(msg);
        }

        foreach (var methodWithOrder in withOrdersList.OrderBy(x => x.TestOrder)) yield return methodWithOrder.Method;
    }
}