using TradeRisk.Domain.Classification;
using TradeRisk.Domain.Classification.Interfaces;
using TradeRisk.Domain.Classification.Rules;
using TradeRisk.Domain.Models;
using TradeRisk.Domain.Models.Enums;
using Xunit;

public class ClassifierTests
{
    private readonly TradeRiskClassifier _classifier = new(new IRiskRule[]
    {
        new LowRiskRule(),
        new MediumRiskRule(),
        new HighRiskRule()
    });

    [Theory]
    [InlineData(500_000, "Public", RiskCategory.LOWRISK)]
    [InlineData(1_000_000, "Public", RiskCategory.MEDIUMRISK)]
    [InlineData(2_000_000, "Private", RiskCategory.HIGHRISK)]
    public void Classify_ReturnsExpectedCategory(decimal value, string sector, RiskCategory expected)
    {
        var trade = new Trade { Value = value, ClientSector = sector };
        var result = _classifier.Classify(trade);
        Assert.Equal(expected, result);
    }
}
