using DigitalExchangeService.Instruments;

namespace DigitalExchangeService.Extensions;

public class AmountExtension
{
    /// <summary>
    /// Выходит ли заданное значение за границы
    /// </summary>
    /// <param name="cashFlowAmountInput"></param>
    /// <param name="cashFlowCurrencyInput"></param>
    /// <param name="currencyLimitDict"></param>
    /// <returns></returns>
    public static bool AmountOutOfBorders(decimal cashFlowAmountInput, Currency cashFlowCurrencyInput,
        CurrencyLimited currencyLimitDict)
    {
        if (currencyLimitDict != null && currencyLimitDict.TryGetValue(cashFlowCurrencyInput.ToString(), out var limit))
        {
            return cashFlowAmountInput < limit.MinAllowToTransfer ||
                   cashFlowAmountInput > limit.MaxAllowToTransfer;
        }

        return false;
    }

    public static bool NotPositive(decimal cashFlowAmountInput) => cashFlowAmountInput <= 0;
}