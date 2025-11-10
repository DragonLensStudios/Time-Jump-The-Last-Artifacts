using PXE.Core.Inventory.Items;

namespace PXE.Core.Inventory.Interfaces
{
    public interface IStandardCurrency
    {
        CurrencyContainer Copper { get; set; }
        CurrencyContainer Silver { get; set; }
        CurrencyContainer Gold { get; set; }
        CurrencyContainer Platinum { get; set; }

        void SetUpCurrency();
        void AddCopperValue(long value);
        void AddSilverValue(long value);
        void AddGoldValue(long value);
        void AddPlatinumValue(long value);
        void SubtractCopperValue(long value);
        void SubtractSilverValue(long value);
        void SubtractGoldValue(long value);
        void SubtractPlatinumValue(long value);
        void MultiplyCopperValue(long value);
        void MultiplySilverValue(long value);
        void MultiplyGoldValue(long value);
        void MultiplyPlatinumValue(long value);
        void DivideCopperValue(long value);
        void DivideSilverValue(long value);
        void DivideGoldValue(long value);
        void DividePlatinumValue(long value);
        void SetCopper(long value);
        void SetSilver(long value);
        void SetGold(long value);
        void SetPlatinum(long value);
        void ResetCopperValue();
        void ResetSilverValue();
        void ResetGoldValue();
        void ResetPlatinumValue();
        void ResetAllValues();
        void DistributeValues();
    }
}