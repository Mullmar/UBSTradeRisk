using System;
using System.Collections.Generic;
using System.Text;

namespace TradeRisk.Application.Dtos
{
    public sealed class AnalyzeResponseDto
    {
        public string[] Categories { get; init; } = Array.Empty<string>();
        public Dictionary<string, RiskSummaryDto> Summary { get; init; } = new();
        public long ProcessingTimeMs { get; init; }
    }

    public sealed class RiskSummaryDto
    {
        public int Count { get; init; }
        public decimal TotalValue { get; init; }
        public string? TopClient { get; init; }
    }
}
