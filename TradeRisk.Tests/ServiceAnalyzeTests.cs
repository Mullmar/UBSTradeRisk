// tests/TradeRisk.Tests/Application/ServiceAnalyzeTests.cs
using TradeRisk.Application.Dtos;
using TradeRisk.Application.Services;
using TradeRisk.Domain.Classification;
using TradeRisk.Domain.Classification.Interfaces;
using TradeRisk.Domain.Classification.Rules;
using Xunit;

public class ServiceAnalyzeTests
{
    private readonly ITradeRiskService _service;

    public ServiceAnalyzeTests()
    {
        var classifier = new TradeRiskClassifier(new IRiskRule[]
        {
            new LowRiskRule(),
            new MediumRiskRule(),
            new HighRiskRule()
        });
        _service = new TradeRiskService(classifier);
    }

    [Fact]
    public void Analyze_ComputesSummaryAndTopClient()
    {
        var trades = new[]
        {
            new TradeRequestDto { Value = 2_000_000, ClientSector = "Private", ClientId = "CLI003" },
            new TradeRequestDto { Value = 400_000, ClientSector = "Public", ClientId = "CLI001" },
            new TradeRequestDto { Value = 500_000, ClientSector = "Public", ClientId = "CLI001" },
            new TradeRequestDto { Value = 3_000_000, ClientSector = "Public", ClientId = "CLI002" },
        };

        var result = _service.Analyze(trades);

        Assert.Equal(new[] { "HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK" }, result.Categories);

        var low = result.Summary["LOWRISK"];
        Assert.Equal(2, low.Count);
        Assert.Equal(900_000m, low.TotalValue);
        Assert.Equal("CLI001", low.TopClient);

        var med = result.Summary["MEDIUMRISK"];
        Assert.Equal(1, med.Count);
        Assert.Equal(3_000_000m, med.TotalValue);
        Assert.Equal("CLI002", med.TopClient);

        var high = result.Summary["HIGHRISK"];
        Assert.Equal(1, high.Count);
        Assert.Equal(2_000_000m, high.TotalValue);
        Assert.Equal("CLI003", high.TopClient);
    }
}
