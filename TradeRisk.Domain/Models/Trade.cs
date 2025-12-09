using System;
using System.Collections.Generic;
using System.Text;

namespace TradeRisk.Domain.Models
{
    public sealed class Trade
    {
        public decimal Value { get; init; }
        public string ClientSector { get; init; } = default!;
        public string? ClientId { get; init; }
    }
}
