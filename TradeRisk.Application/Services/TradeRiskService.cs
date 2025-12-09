using System.Diagnostics;
using TradeRisk.Application.Dtos;
using TradeRisk.Domain.Classification;
using TradeRisk.Domain.Models;
using TradeRisk.Domain.Models.Enums;

namespace TradeRisk.Application.Services;

public interface ITradeRiskService
{
    ClassifyResponseDto Classify(IEnumerable<TradeRequestDto> trades);
    AnalyzeResponseDto Analyze(IEnumerable<TradeRequestDto> trades);
}

public sealed class TradeRiskService : ITradeRiskService
{
    private readonly TradeRiskClassifier _classifier;

    public TradeRiskService(TradeRiskClassifier classifier)
    {
        _classifier = classifier;
    }

    public ClassifyResponseDto Classify(IEnumerable<TradeRequestDto> trades)
    {
        var categories = trades.Select(MapAndClassify)
                               .Select(c => c.ToString())
                               .ToArray();
        return new ClassifyResponseDto { Categories = categories };
    }

    public AnalyzeResponseDto Analyze(IEnumerable<TradeRequestDto> trades)
    {
        var sw = Stopwatch.StartNew();

        // Processamento eficiente para até 100k trades
        var categoriesList = new List<string>();
        var aggByCategory = new Dictionary<RiskCategory, CategoryAggregation>(capacity: 3);

        foreach (var dto in trades)
        {
            var category = MapAndClassify(dto);
            categoriesList.Add(category.ToString());

            if (!aggByCategory.TryGetValue(category, out var agg))
            {
                agg = new CategoryAggregation();
                aggByCategory[category] = agg;
            }

            agg.Count++;
            agg.TotalValue += dto.Value;

            if (!string.IsNullOrWhiteSpace(dto.ClientId))
            {
                var clientTotals = agg.ClientTotals ??= new Dictionary<string, decimal>();
                clientTotals[dto.ClientId] = clientTotals.TryGetValue(dto.ClientId, out var sum)
                    ? sum + dto.Value
                    : dto.Value;
            }
        }

        var summary = aggByCategory.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => new RiskSummaryDto
            {
                Count = kvp.Value.Count,
                TotalValue = kvp.Value.TotalValue,
                TopClient = kvp.Value.ClientTotals is null || kvp.Value.ClientTotals.Count == 0
                    ? null
                    : kvp.Value.ClientTotals.MaxBy(x => x.Value).Key
            });

        sw.Stop();

        return new AnalyzeResponseDto
        {
            Categories = categoriesList.ToArray(),
            Summary = summary,
            ProcessingTimeMs = sw.ElapsedMilliseconds
        };
    }

    private RiskCategory MapAndClassify(TradeRequestDto dto)
    {
        var trade = new Trade
        {
            Value = dto.Value,
            ClientSector = dto.ClientSector,
            ClientId = dto.ClientId
        };
        return _classifier.Classify(trade);
    }

    private sealed class CategoryAggregation
    {
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
        public Dictionary<string, decimal>? ClientTotals { get; set; }
    }
}
