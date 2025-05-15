using Microsoft.AspNetCore.Mvc;

namespace CurrencyMockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private static readonly Dictionary<string, decimal> ExchangeRates = new()
        {
            {"USD", 1.00m},
            {"EUR", 0.93m},
            {"GBP", 0.80m},
            {"JPY", 151.67m},
            {"CAD", 1.37m},
            {"AUD", 1.52m},
            {"CNY", 7.24m},
            {"RUB", 93.45m}
        };

        [HttpGet("rates")]
        public IActionResult GetRates()
        {
            return Ok(ExchangeRates);
        }

        [HttpGet("rate/{currencyCode}")]
        public IActionResult GetRate(string currencyCode)
        {
            if (ExchangeRates.TryGetValue(currencyCode.ToUpper(), out var rate))
            {
                return Ok(new { Currency = currencyCode.ToUpper(), Rate = rate });
            }

            return NotFound($"Currency code {currencyCode} not found");
        }

        [HttpPost("update")]
        public IActionResult UpdateRate([FromBody] CurrencyUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                return BadRequest("Currency code is required");
            }

            var currencyCode = request.CurrencyCode.ToUpper();
            ExchangeRates[currencyCode] = request.NewRate;

            return Ok(new { Message = "Rate updated successfully", Currency = currencyCode, NewRate = request.NewRate });
        }
    }

    public class CurrencyUpdateRequest
    {
        public string? CurrencyCode { get; set; }
        public decimal NewRate { get; set; }
    }
}