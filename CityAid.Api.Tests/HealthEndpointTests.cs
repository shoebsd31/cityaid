using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityAid.Api.Tests;

[TestClass]
public class HealthEndpointTests
{
    [TestMethod]
    public async Task Health_Returns_200()
    {
        await using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();
        var resp = await client.GetAsync("/health");
        Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
}