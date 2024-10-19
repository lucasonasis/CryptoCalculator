namespace CryptoCalculator.Models
{
    public class PriceData
    {
        public int Id { get; set; }
        public int CryptoCurrencyId { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}
