using RapidIntegrationTesting.xUnit;

namespace RapidIntegrationTesting.Integration.Tests.Tests;

[TestCaseOrderer(TestOrderer.TypeName, TestOrderer.AssemblyName)]
public class TestOrdererTests
{
    private static bool _testOneRun;
    private static bool _testTwoRun;
    private static bool _testThreeRun;
    private static bool _testFourRun;

    [Fact]
    [TestOrder(3)]
    public void Three()
    {
        Assert.True(_testOneRun);
        Assert.True(_testTwoRun);
        Assert.False(_testThreeRun);
        Assert.False(_testFourRun);

        _testThreeRun = true;
    }

    [Fact]
    [TestOrder(1)]
    public void One()
    {

        Assert.False(_testOneRun);
        Assert.False(_testTwoRun);
        Assert.False(_testThreeRun);
        Assert.False(_testFourRun);

        _testOneRun = true;
    }

    [Fact]
    [TestOrder(4)]
    public void Four()
    {
        Assert.True(_testOneRun);
        Assert.True(_testTwoRun);
        Assert.True(_testThreeRun);
        Assert.False(_testFourRun);

        _testFourRun = true;
    }

    [Fact]
    [TestOrder(2)]
    public void Two()
    {
        Assert.True(_testOneRun);
        Assert.False(_testTwoRun);
        Assert.False(_testThreeRun);
        Assert.False(_testFourRun);

        _testTwoRun = true;
    }
}