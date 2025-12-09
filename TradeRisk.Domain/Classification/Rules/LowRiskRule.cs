using System;
using System.Collections.Generic;
using System.Text;
using TradeRisk.Domain.Classification.Interfaces;
using TradeRisk.Domain.Models;
using TradeRisk.Domain.Models.Enums;

namespace TradeRisk.Domain.Classification.Rules
{
    public sealed class LowRiskRule : IRiskRule
    {
        public RiskCategory Category => RiskCategory.LOWRISK;

        public bool CanApply(Trade trade)
        {
            return trade.Value < 1_000_000m;
        }
    }
}
