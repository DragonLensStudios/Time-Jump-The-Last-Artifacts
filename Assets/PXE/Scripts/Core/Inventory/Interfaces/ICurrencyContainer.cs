using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Core.Inventory.Interfaces
{
    public interface ICurrencyContainer
    {
        CurrencyType Type { get; set; }
        long Value { get; set; }
        bool IsTieredCurrency { get; set; }
        long Threshold { get; set; }
        Sprite CurrencyIcon { get; set; }
        bool HasLimit { get; set; }
        long Limit { get; set; }
        void AddValue(long value);
        void SubtractValue(long value);
        void MultiplyValue(long value);
        void DivideValue(long value);
        void SetValue(long value);
        void ResetValue();
    }
}