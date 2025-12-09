using System;
using System.Collections.Generic;
using System.Text;
using TradeRisk.Domain.Models;
using TradeRisk.Domain.Models.Enums;

namespace TradeRisk.Domain.Classification.Interfaces
{
    public interface IRiskRule
    {
        bool CanApply(Trade trade);
        RiskCategory Category { get; }
    }
}
