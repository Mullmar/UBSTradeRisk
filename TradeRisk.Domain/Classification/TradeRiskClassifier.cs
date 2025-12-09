using System;
using System.Collections.Generic;
using System.Text;
using TradeRisk.Domain.Classification.Interfaces;
using TradeRisk.Domain.Models;
using TradeRisk.Domain.Models.Enums;

namespace TradeRisk.Domain.Classification
{
    public sealed class TradeRiskClassifier
    {
        private readonly IReadOnlyList<IRiskRule> _rules;

        public TradeRiskClassifier(IEnumerable<IRiskRule> rules) 
        {
            _rules = rules.ToList();
        }

        public RiskCategory Classify(Trade trade) 
        {
            if (_rules.Count == 0)
                throw new ArgumentException("Pelo menos uma regra precisa ser fornecida.");

            foreach (var rule in _rules) 
            {
                if (rule.CanApply(trade)) 
                { 
                    return rule.Category; 
                }
            }
            //Fallback: se nenhuma regra bater, retorna LowRisk
            return RiskCategory.LOWRISK;
        }
    }
}
