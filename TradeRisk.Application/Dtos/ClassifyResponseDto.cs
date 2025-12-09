using System;
using System.Collections.Generic;
using System.Text;

namespace TradeRisk.Application.Dtos
{
    public sealed class ClassifyResponseDto
    {
        public string[] Categories { get; init; } = Array.Empty<string>();
    }
}
