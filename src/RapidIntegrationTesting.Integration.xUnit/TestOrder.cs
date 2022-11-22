namespace RapidIntegrationTesting.Integration.xUnit;

/// <summary>
///     Attribute used to indicate the order for test execution
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestOrderAttribute : Attribute
{
    /// <summary>
    ///     Executes a test in the given order
    /// </summary>
    /// <param name="order"></param>
    public TestOrderAttribute(uint order)
    {
        Order = order;
    }

    /// <summary>
    ///     Test Execution Order
    /// </summary>
    public uint Order { get; }
}