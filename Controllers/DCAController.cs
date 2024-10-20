using CryptoCalculator.Models;
using CryptoCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DCAController : ControllerBase
    {
        private readonly DCAService _dcaService;

        public DCAController(DCAService dcaService)
        {
            _dcaService = dcaService;
        }

        [HttpGet("currencies")]
        public async Task<ActionResult<List<CryptoCurrency>>> Get()
        {
            var cryptos = await _dcaService.GetAllCryptoCurrenciesAsync();
            return Ok(cryptos);
        }

        [HttpGet("calculate")]
        public async Task<ActionResult<List<InvestmentData>>> Calculate(
            [FromQuery] List<int> cryptoIds,
            [FromQuery] List<DateTime> startDates,
            [FromQuery] List<decimal> monthlyInvestments,
            [FromQuery] DateTime endDate)
        {
            if (cryptoIds.Count != startDates.Count || cryptoIds.Count != monthlyInvestments.Count)
            {
                return BadRequest("Mismatched parameter counts.");
            }

            var investmentPlans = new List<InvestmentPlan>();
            for (int i = 0; i < cryptoIds.Count; i++)
            {
                investmentPlans.Add(new InvestmentPlan
                {
                    CryptoId = cryptoIds[i],
                    StartDate = startDates[i],
                    MonthlyInvestment = monthlyInvestments[i]
                });
            }

            var investmentData = await _dcaService.CalculateDCAInvestment(investmentPlans, endDate);
            return Ok(investmentData);
        }

        [HttpGet("calculate-multiple")]
        public async Task<ActionResult<List<InvestmentData>>> CalculateForMultipleCryptos(
            [FromQuery] List<int> cryptoIds,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] decimal monthlyInvestment,
            [FromQuery] List<decimal> investmentShares)
        {
            if (cryptoIds == null || !cryptoIds.Any())
            {
                return BadRequest("CryptoIds cannot be null or empty.");
            }

            if (cryptoIds.Count != investmentShares.Count)
            {
                return BadRequest("Mismatched parameter counts.");
            }

            var investmentData = await _dcaService.CalculateDCAInvestmentForMultipleCryptos(
                cryptoIds, startDate, endDate, monthlyInvestment, investmentShares);

            return Ok(investmentData);
        }
    }
}