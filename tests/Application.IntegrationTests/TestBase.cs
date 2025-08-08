using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests;

public abstract class TestBase
{
    [SetUp]
    public void TestSetUp()
    {
        Testing.ResetState();
    }
}