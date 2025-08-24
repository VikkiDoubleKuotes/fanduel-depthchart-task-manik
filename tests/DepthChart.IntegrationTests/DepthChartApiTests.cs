using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using DomainPlayer = Mani.Fanduel.Task.Player;

namespace Mani.Fanduel.Task.IntegrationTests;

public class DepthChartApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DepthChartApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
public async System.Threading.Tasks.Task Seed_Then_GetBackups_Works()
    {
        await _client.PostAsync("/seed", null);

        var backups = await _client.GetFromJsonAsync<DomainPlayer[]>("/positions/QB/backups/12");
        backups!.Select(p => p.Number.Value).Should().Equal(11, 2);
    }
}

