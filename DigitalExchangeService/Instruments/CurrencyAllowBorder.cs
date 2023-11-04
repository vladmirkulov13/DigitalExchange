namespace DigitalExchangeService.Instruments;

/// <summary>
/// Допустимые границы для перевода в конкретной валюте
/// </summary>
public class CurrencyAllowBorder
{
    public decimal MinAllowToTransfer { get; set; }
    
    public decimal MaxAllowToTransfer { get; set; }
}