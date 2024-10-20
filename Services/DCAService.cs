using CryptoCalculator.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCalculator.Services
{
    public class DCAService
    {
        private readonly AppDbContext _dbContext;

        public DCAService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CryptoCurrency>> GetAllCryptoCurrenciesAsync()
        {
            return await _dbContext.CryptoCurrencies.ToListAsync();
        }

        public async Task<List<InvestmentData>> CalculateDCAInvestment(List<InvestmentPlan> investmentPlans, DateTime endDate)
        {
            var investmentDataList = new List<InvestmentData>();

            foreach (var plan in investmentPlans)
            {
                DateTime currentDate = plan.StartDate;
                decimal totalInvested = 0;
                decimal totalCryptoAmount = 0;

                // Fetch the latest available price for the cryptocurrency
                var latestPriceData = await _dbContext.PriceData
                    .Where(p => p.CryptoCurrencyId == plan.CryptoId)
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefaultAsync();

                if (latestPriceData == null)
                {
                    Console.WriteLine($"No latest price data found for {plan.CryptoId}");
                    continue;
                }

                decimal latestPrice = latestPriceData.Price;

                while (currentDate <= endDate)
                {
                    // Fetch the price for the current date
                    var priceData = await _dbContext.PriceData
                        .Where(p => p.CryptoCurrencyId == plan.CryptoId && p.Date.Date == currentDate.Date)
                        .FirstOrDefaultAsync();

                    if (priceData == null)
                    {
                        Console.WriteLine($"No price data found for {plan.CryptoId} on {currentDate}");
                        currentDate = currentDate.AddMonths(1);
                        continue;
                    }

                    decimal price = priceData.Price;

                    // Calculate the amount of cryptocurrency bought
                    decimal cryptoAmount = plan.MonthlyInvestment / price;

                    // Update totals
                    totalInvested += plan.MonthlyInvestment;
                    totalCryptoAmount += cryptoAmount;

                    // Calculate the value today and ROI
                    decimal valueToday = totalCryptoAmount * latestPrice;
                    decimal roi = (valueToday - totalInvested) / totalInvested * 100;

                    // Add the data to the list
                    investmentDataList.Add(new InvestmentData
                    {
                        Date = currentDate,
                        InvestedAmount = totalInvested,
                        CryptoAmount = totalCryptoAmount,
                        ValueToday = valueToday,
                        ROI = roi
                    });

                    // Move to the next month
                    currentDate = currentDate.AddMonths(1);
                }
            }

            return investmentDataList;
        }


        public async Task<List<InvestmentData>> CalculateDCAInvestmentForMultipleCryptos(
          List<int> cryptoIds, DateTime startDate, DateTime endDate, decimal monthlyInvestment, List<decimal> investmentShares)
        {
            var investmentDataList = new List<InvestmentData>();
            DateTime currentDate = startDate;
            decimal totalInvested = 0;
            var totalCryptoAmounts = new Dictionary<int, decimal>();

            // Initialize totalCryptoAmounts for each cryptoId
            foreach (var cryptoId in cryptoIds)
            {
                totalCryptoAmounts[cryptoId] = 0;
            }

            while (currentDate <= endDate)
            {
                decimal monthlyTotalInvestment = 0;

                for (int i = 0; i < cryptoIds.Count; i++)
                {
                    if (currentDate < startDate)
                        continue;

                    // Fetch the price for the current date
                    var priceData = await _dbContext.PriceData
                        .Where(p => p.CryptoCurrencyId == cryptoIds[i] && p.Date.Date == currentDate.Date)
                        .FirstOrDefaultAsync();

                    if (priceData == null)
                    {
                        Console.WriteLine($"No price data found for {cryptoIds[i]} on {currentDate}");
                        continue;
                    }

                    decimal price = priceData.Price;
                    decimal cryptoInvestment = monthlyInvestment * investmentShares[i] / 100;

                    // Calculate the amount of cryptocurrency bought
                    decimal cryptoAmount = cryptoInvestment / price;

                    // Update monthly totals
                    monthlyTotalInvestment += cryptoInvestment;
                    totalCryptoAmounts[cryptoIds[i]] += cryptoAmount;
                }

                // Update overall totals
                totalInvested += monthlyTotalInvestment;

                // Fetch the latest available price for each cryptocurrency and calculate the combined value today
                decimal combinedValueToday = 0;
                for (int i = 0; i < cryptoIds.Count; i++)
                {
                    var latestPriceData = await _dbContext.PriceData
                        .Where(p => p.CryptoCurrencyId == cryptoIds[i])
                        .OrderByDescending(p => p.Date)
                        .FirstOrDefaultAsync();

                    if (latestPriceData == null)
                    {
                        Console.WriteLine($"No latest price data found for {cryptoIds[i]}");
                        continue;
                    }

                    decimal latestPrice = latestPriceData.Price;
                    combinedValueToday += totalCryptoAmounts[cryptoIds[i]] * latestPrice;
                }

                // Calculate the ROI
                decimal roi = (combinedValueToday - totalInvested) / totalInvested * 100;

                // Add the data to the list
                investmentDataList.Add(new InvestmentData
                {
                    Date = currentDate,
                    InvestedAmount = totalInvested,
                    CryptoAmount = totalCryptoAmounts.Values.Sum(),
                    ValueToday = combinedValueToday,
                    ROI = roi
                });

                // Move to the next month
                currentDate = currentDate.AddMonths(1);
            }

            return investmentDataList;
        }
    }
}