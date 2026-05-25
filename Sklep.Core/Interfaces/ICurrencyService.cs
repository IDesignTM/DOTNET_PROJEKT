namespace Sklep.Core.Interfaces;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string currencyCode);
}