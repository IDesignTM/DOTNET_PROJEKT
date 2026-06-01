using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Interfaces;
using Sklep.Core.Models;
using Sklep.Infrastructure.Data;

namespace Sklep.Infrastructure.Services;

public class NbpService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;

    public NbpService(HttpClient httpClient, ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }
    
    public async Task<decimal?> GetExchangeRateAsync(string currencyCode)
    {
        currencyCode = currencyCode.ToUpper();
        
        var cachedRate = await _context.CurrencyExchangeRates
            .Where(r => r.CurrencyCode == currencyCode)
            .OrderByDescending(r => r.FetchedAt)
            .FirstOrDefaultAsync();
        
        if (cachedRate != null && cachedRate.FetchedAt > DateTime.UtcNow.AddHours(-12))
        {
            return cachedRate.Rate;
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            
            var url = $"https://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/?format=json";
            
            var response = await _httpClient.GetFromJsonAsync<NbpResponse>(url, cts.Token);
            
            if (response?.Rates != null && response.Rates.Any())
            {
                var newRateValue = (decimal)response.Rates.First().Mid;

                var rateEntity = cachedRate ?? new CurrencyExchangeRate { CurrencyCode = currencyCode };
                rateEntity.Rate = newRateValue;
                rateEntity.FetchedAt = DateTime.UtcNow;

                if (rateEntity.Id == 0)
                    _context.CurrencyExchangeRates.Add(rateEntity);
                else
                    _context.CurrencyExchangeRates.Update(rateEntity);

                await _context.SaveChangesAsync();
                return newRateValue;
            }
        }
        catch (Exception)
        {
            if (cachedRate != null) return cachedRate.Rate;
        }
        
        return null;
    }
}

public class NbpResponse
{
    [JsonPropertyName("rates")]
    public List<NbpRate> Rates { get; set; } = new List<NbpRate>();
}

public class NbpRate
{
    [JsonPropertyName("mid")]
    public double Mid { get; set; }
}