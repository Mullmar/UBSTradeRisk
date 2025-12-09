using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TradeRisk.Application.Dtos
{
    public sealed class TradeRequestDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo.")]
        public decimal Value { get; init; }

        [Required]
        [RegularExpression("^(Public|Private)$", ErrorMessage = "Setor do cliente deve ser 'Public' ou 'Private'.")]
        public string ClientSector { get; init; } = default!;

        public string? ClientId { get; init; }
    }
}
