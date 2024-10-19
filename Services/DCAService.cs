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
                    // Ensure the date is one of the valid dates (15th, 20th, 25th)
                    if (currentDate.Day != plan.InvestmentDay)
                    {
                        currentDate = currentDate.AddMonths(1);
                        continue;
                    }

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
    }


}