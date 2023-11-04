using System.Collections.Generic;
using DigitalExchangeService.Instruments;

namespace DigitalExchangeService.Models;

/// <summary>
/// Счет пользователя
/// </summary>
public class Account
{
    /// <summary>
    /// Словарь (валюта-сумма остатка)
    /// </summary>
    public Dictionary<Currency, decimal> CurrencyAmount { get; set; }
}