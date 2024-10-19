using CryptoCalculator.Models;
using CryptoCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    }
}