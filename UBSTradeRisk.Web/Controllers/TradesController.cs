using Microsoft.AspNetCore.Mvc;
using TradeRisk.Application.Dtos;
using TradeRisk.Application.Services;

namespace TradeRisk.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TradesController : ControllerBase
{
    private readonly ITradeRiskService _service;
    private readonly ILogger<TradesController> _logger;

    public TradesController(ITradeRiskService service, ILogger<TradesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("classify")]
    public ActionResult<ClassifyResponseDto> Classify([FromBody] List<TradeRequestDto> trades)
    {
        _logger.LogInformation("Classifique requisições com {Count} trades", trades?.Count ?? 0);

        if (trades is null || trades.Count == 0)
            return BadRequest(new { message = "Lista de entrada não deve ser vazia." });

        var result = _service.Classify(trades);
        return Ok(result);
    }

    [HttpPost("analyze")]
    public ActionResult<AnalyzeResponseDto> Analyze([FromBody] List<TradeRequestDto> trades)
    {
        _logger.LogInformation("Analise requisições com {Count} trades", trades?.Count ?? 0);

        if (trades is null || trades.Count == 0)
            return BadRequest(new { message = "Lista de entrada não deve ser vazia." });

        var result = _service.Analyze(trades);
        return Ok(result);
    }
}
