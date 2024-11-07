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
                await ProcessInvestmentPlan(plan, endDate, investmentDataList);
            }

            return investmentDataList;
        }

        public async Task<List<InvestmentData>> CalculateDCAInvestmentForMultipleCryptos(
            List<int> cryptoIds, DateTime startDate, DateTime endDate, decimal monthlyInvestment, List<decimal> investmentShares)
        {
            var investmentDataList = new List<InvestmentData>();
            DateTime currentDate = startDate;
            decimal totalInvested = 0;
            var totalCryptoAmounts = cryptoIds.ToDictionary(id => id, id => 0m);

            while (currentDate <= endDate)
            {
                decimal monthlyTotalInvestment = 0;

                for (int i = 0; i < cryptoIds.Count; i++)
                {
                    if (currentDate < startDate)
                        continue;

                    var priceData = await GetPriceDataForDate(cryptoIds[i], currentDate);
                    if (priceData == null)
                    {
                        Console.WriteLine($"No price data found for {cryptoIds[i]} on {currentDate}");
                        continue;
                    }

                    decimal cryptoInvestment = monthlyInvestment * investmentShares[i] / 100;
                    decimal cryptoAmount = cryptoInvestment / priceData.Price;

                    monthlyTotalInvestment += cryptoInvestment;
                    totalCryptoAmounts[cryptoIds[i]] += cryptoAmount;
                }

                totalInvested += monthlyTotalInvestment;
                decimal combinedValueToday = await CalculateCombinedValueToday(cryptoIds, totalCryptoAmounts);

                decimal roi = (combinedValueToday - totalInvested) / totalInvested * 100;

                investmentDataList.Add(new InvestmentData
                {
                    Date = currentDate,
                    InvestedAmount = totalInvested,
                    CryptoAmount = totalCryptoAmounts.Values.Sum(),
                    ValueToday = combinedValueToday,
                    ROI = roi
                });

                currentDate = currentDate.AddMonths(1);
            }

            return investmentDataList;
        }



        private async Task ProcessInvestmentPlan(InvestmentPlan plan, DateTime endDate, List<InvestmentData> investmentDataList)
        {
            DateTime? currentDate = plan.StartDate;
            decimal totalInvested = 0;
            decimal totalCryptoAmount = 0;

            var latestPriceData = await GetLatestPriceData(plan.CryptoId);
            if (latestPriceData == null)
            {
                Console.WriteLine($"No latest price data found for {plan.CryptoId}");
                return;
            }

            decimal latestPrice = latestPriceData.Price;

            while (currentDate <= endDate)
            {
                var priceData = await GetPriceDataForDate(plan.CryptoId, currentDate.Value);
                if (priceData == null)
                {
                    Console.WriteLine($"No price data found for {plan.CryptoId} on {currentDate}");
                    currentDate = currentDate?.AddMonths(1);
                    continue;
                }

                decimal cryptoAmount = plan.MonthlyInvestment / priceData.Price;
                totalInvested += plan.MonthlyInvestment;
                totalCryptoAmount += cryptoAmount;

                decimal valueToday = totalCryptoAmount * latestPrice;
                decimal roi = (valueToday - totalInvested) / totalInvested * 100;

                investmentDataList.Add(new InvestmentData
                {
                    Date = currentDate,
                    InvestedAmount = totalInvested,
                    CryptoAmount = totalCryptoAmount,
                    ValueToday = valueToday,
                    ROI = roi
                });

                currentDate = currentDate?.AddMonths(1);
            }
        }

        private async Task<PriceData> GetPriceDataForDate(int cryptoId, DateTime date)
        {
            return await _dbContext.PriceData
                .Where(p => p.CryptoCurrencyId == cryptoId && p.Date.Date == date.Date)
                .FirstOrDefaultAsync();
        }

        private async Task<PriceData> GetLatestPriceData(int cryptoId)
        {
            return await _dbContext.PriceData
                .Where(p => p.CryptoCurrencyId == cryptoId)
                .OrderByDescending(p => p.Date)
                .FirstOrDefaultAsync();
        }

        private async Task<decimal> CalculateCombinedValueToday(List<int> cryptoIds, Dictionary<int, decimal> totalCryptoAmounts)
        {
            decimal combinedValueToday = 0;

            foreach (var cryptoId in cryptoIds)
            {
                var latestPriceData = await GetLatestPriceData(cryptoId);
                if (latestPriceData == null)
                {
                    Console.WriteLine($"No latest price data found for {cryptoId}");
                    continue;
                }

                combinedValueToday += totalCryptoAmounts[cryptoId] * latestPriceData.Price;
            }

            return combinedValueToday;
        }
    }
}