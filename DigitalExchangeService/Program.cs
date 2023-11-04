using System;
using System.Collections.Generic;
using System.IO;
using DigitalExchangeService.Extensions;
using DigitalExchangeService.Instruments;
using DigitalExchangeService.Models;
using Microsoft.Extensions.Configuration;

namespace DigitalExchangeService;

/// <summary>
/// Списываем с одной валюты, конвертируем в другую
/// </summary>
internal static class Program
{
    private static decimal _defCurrencyAmount;

    private static decimal _ABRate;

    private static decimal _commission;

    private static CurrencyLimited _currencyLimitedDict;
    
    public static void Main(string[] args)
    {
        BuildConfig();

        Console.WriteLine($"DefaultCurrencyAmount: {_defCurrencyAmount}");
        Console.WriteLine($"Commission: {_commission}");
        Console.WriteLine($"ABRate: {_ABRate}");

        var userAccount = new Account();
        var earnedCommission = 0m;
        
        while (true)
        {
            Console.WriteLine("Валюта списания (A, B): ");

            if (!Enum.TryParse<Currency>(Console.ReadLine(), out var cashFlowCurrencyInput))
            {
                Console.WriteLine("Введена некорректная валюта");
                continue;
            }

            var anotherCurrency = cashFlowCurrencyInput == Currency.A ? Currency.B : Currency.A;

            if (userAccount.CurrencyAmount is null ||
                !userAccount.CurrencyAmount.TryGetValue(cashFlowCurrencyInput, out var currentAmount))
            {
                userAccount.CurrencyAmount = new Dictionary<Currency, decimal>
                    { { cashFlowCurrencyInput, _defCurrencyAmount } };
                currentAmount = _defCurrencyAmount;
                if (!userAccount.CurrencyAmount.TryGetValue(anotherCurrency, out _))
                {
                    userAccount.CurrencyAmount[anotherCurrency] = _defCurrencyAmount;
                }
            }

            Console.WriteLine($"На счету в валюте {cashFlowCurrencyInput}: {Math.Round(currentAmount, 2)}");

            Console.WriteLine("Сумма списания: ");
            if (!decimal.TryParse(Console.ReadLine(), out var cashFlowAmountInput))
            {
                Console.WriteLine("Некорректная сумма списания");
                continue;
            }

            if (AmountExtension.NotPositive(cashFlowAmountInput))
            {
                Console.WriteLine("Сумма списания должна быть положительной");
                continue;
            }
            if (cashFlowAmountInput > currentAmount)
            {
                Console.WriteLine("Сумма списания превышает остаток на счете");
                continue;
            }

            if (AmountExtension.AmountOutOfBorders(cashFlowAmountInput, cashFlowCurrencyInput, _currencyLimitedDict))
            {
                Console.WriteLine("Сумма списания не входит в допустимые границы");
                Console.WriteLine("Границы: ");
                if (_currencyLimitedDict != null)
                    foreach (var limit in _currencyLimitedDict)
                    {
                        Console.WriteLine($"Валюта {limit.Key}: ");
                        Console.WriteLine($"MIN: {limit.Value.MinAllowToTransfer}");
                        Console.WriteLine($"MAX: {limit.Value.MaxAllowToTransfer}");
                    }

                continue;
            }


            userAccount.CurrencyAmount[cashFlowCurrencyInput] -= cashFlowAmountInput;

            userAccount.CurrencyAmount[anotherCurrency] += AmountCurrencyConverter.ConvertInputAmount(
                cashFlowAmountInput, cashFlowCurrencyInput,
                _ABRate, _commission, ref earnedCommission);

            Console.WriteLine("Текущее состояние счета:");

            Console.WriteLine(
                $"Валюта {cashFlowCurrencyInput}: {Math.Round(userAccount.CurrencyAmount[cashFlowCurrencyInput], 2)}");

            Console.WriteLine(
                $"Валюта {anotherCurrency}: {Math.Round(userAccount.CurrencyAmount[anotherCurrency], 2)}");

            Console.WriteLine($"Заработано на комиссии: {Math.Round(earnedCommission, 2)}");
        }
    }

    private static void BuildConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();

        _defCurrencyAmount = config.GetSection("DefaultCurrencyAmount").Get<decimal>();
        _commission = config.GetSection("Commission").Get<decimal>();
        _ABRate = config.GetSection("ABRate").Get<decimal>();
        _currencyLimitedDict= config.GetSection("CurrencyLimited").Get<CurrencyLimited>();
    }
   
}