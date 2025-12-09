using System;
using System.Collections.Generic;
using System.Text;
using TradeRisk.Domain.Classification.Interfaces;
using TradeRisk.Domain.Models;
using TradeRisk.Domain.Models.Enums;

namespace TradeRisk.Domain.Classification.Rules
{
    public sealed class MediumRiskRule : IRiskRule
    {
        public RiskCategory Category => RiskCategory.MEDIUMRISK;

        public bool CanApply(Trade trade)
        {
            return trade.Value >= 1_000_000m &&
                string.Equals(trade.ClientSector, "Public", StringComparison.OrdinalIgnoreCase);
        }
    }
}
