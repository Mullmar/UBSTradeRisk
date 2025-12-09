// tests/TradeRisk.Tests/Web/TradesControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TradeRisk.Application.Dtos;
using Xunit;

public class TradesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TradesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task Classify_ReturnsCategories()
    {
        var client = _factory.CreateClient();

        var input = new[]
        {
            new TradeRequestDto { Value = 2_000_000, ClientSector = "Private" },
            new TradeRequestDto { Value = 400_000, ClientSector = "Public" },
        };

        var response = await client.PostAsJsonAsync("/api/trades/classify", input);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<ClassifyResponseDto>();
        Assert.NotNull(dto);
        Assert.Equal(new[] { "HIGHRISK", "LOWRISK" }, dto!.Categories);
    }

    [Fact]
    public async Task Analyze_ReturnsSummary()
    {
        var client = _factory.CreateClient();

        var input = new[]
        {
            new TradeRequestDto { Value = 2_000_000, ClientSector = "Private", ClientId = "CLI003" },
            new TradeRequestDto { Value = 3_000_000, ClientSector = "Public", ClientId = "CLI002" },
        };

        var response = await client.PostAsJsonAsync("/api/trades/analyze", input);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<AnalyzeResponseDto>();
        Assert.NotNull(dto);
        Assert.Contains("HIGHRISK", dto!.Categories);
        Assert.Contains("MEDIUMRISK", dto.Categories);
        Assert.True(dto.ProcessingTimeMs >= 0);
    }
}
