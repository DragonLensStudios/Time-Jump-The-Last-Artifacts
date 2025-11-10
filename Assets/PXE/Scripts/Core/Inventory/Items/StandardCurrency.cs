using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
  /// <summary>
  ///  This class is used to represent a standard currency in the game.
  /// </summary>
  [System.Serializable]
  public class StandardCurrency
  {
    [field: Tooltip("The copper currency.")]
    [field: SerializeField] public virtual CurrencyContainer Copper { get; set; }
    
    [field: Tooltip("The silver currency.")]
    [field: SerializeField] public virtual CurrencyContainer Silver { get; set; }
    
    [field: Tooltip("The gold currency.")]
    [field: SerializeField] public virtual CurrencyContainer Gold { get; set; }
    
    [field: Tooltip("The platinum currency.")]
    [field: SerializeField] public virtual CurrencyContainer Platinum { get; set; }

    /// <summary>
    ///  Creates a new instance of the StandardCurrency class and sets up the currency.
    /// </summary>
    public StandardCurrency()
    {
      SetUpCurrency();
    }

    /// <summary>
    ///  Sets up the currency with default values.
    /// </summary>
    public virtual void SetUpCurrency()
    {
      Platinum = new CurrencyContainer
      {
        Type = CurrencyType.Platinum,
        Value = 0,
        IsTieredCurrency = false,
        Threshold = 0,
      };
      Gold = new CurrencyContainer
      {
        Type =CurrencyType.Gold,
        Value = 0,
        IsTieredCurrency = true,
        Threshold = 100,
      };
      Silver = new CurrencyContainer
      {
        Type = CurrencyType.Silver,
        Value = 0,
        IsTieredCurrency = true,
        Threshold = 100,
      };
      
      Copper = new CurrencyContainer
      {
        Type = CurrencyType.Copper,
        Value = 0,
        IsTieredCurrency = true,
        Threshold = 100,
      };
    }
    
    /// <summary>
    ///  Adds the given value to the copper currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void AddCopperValue(long value)
    {
      Copper.AddValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Adds the given value to the silver currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void AddSilverValue(long value)
    {
      Silver.AddValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Adds the given value to the gold currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void AddGoldValue(long value)
    {
      Gold.AddValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Adds the given value to the platinum currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void AddPlatinumValue(long value)
    {
      Platinum.AddValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Subtracts the given value from the copper currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SubtractCopperValue(long value)
    {
      Copper.SubtractValue(value);
      DistributeValues();
    }

    /// <summary>
    ///  Subtracts the given value from the silver currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SubtractSilverValue(long value)
    {
      Silver.SubtractValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Subtracts the given value from the gold currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SubtractGoldValue(long value)
    {
      Gold.SubtractValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Subtracts the given value from the platinum currency.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SubtractPlatinumValue(long value)
    {
      Platinum.SubtractValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Multiplies the copper currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void MultiplyCopperValue(long value)
    {
      Copper.MultiplyValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Multiplies the silver currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void MultiplySilverValue(long value)
    {
      Silver.MultiplyValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Multiplies the gold currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void MultiplyGoldValue(long value)
    {
      Gold.MultiplyValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Multiplies the platinum currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void MultiplyPlatinumValue(long value)
    {
      Platinum.MultiplyValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Divides the copper currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void DivideCopperValue(long value)
    {
      Copper.DivideValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Divides the silver currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void DivideSilverValue(long value)
    {
      Silver.DivideValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Divides the gold currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void DivideGoldValue(long value)
    {
      Gold.DivideValue(value);
      DistributeValues();
    }
    
    /// <summary>
    ///  Divides the platinum currency by the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void DividePlatinumValue(long value)
    {
      Platinum.DivideValue(value);
    }
    
    /// <summary>
    ///  Sets the copper currency to the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetCopper(long value)
    {
      ResetAllValues();
      Copper.Value = value;
      DistributeValues();
    }
    
    /// <summary>
    ///  Sets the silver currency to the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetSilver(long value)
    {
      ResetAllValues();
      Silver.Value = value;
      DistributeValues();
    }
    
    /// <summary>
    ///  Sets the gold currency to the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetGold(long value)
    {
      ResetAllValues();
      Gold.Value = value;
      DistributeValues();
    }
    
    /// <summary>
    ///  Sets the platinum currency to the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetPlatinum(long value)
    {
      ResetAllValues();
      Platinum.Value = value;
      DistributeValues();
    }
    
    /// <summary>
    ///  Resets the copper currency to 0.
    /// </summary>
    public virtual void ResetCopperValue()
    {
      Copper.ResetValue();
      DistributeValues();
    }
    
    /// <summary>
    ///  Resets the silver currency to 0.
    /// </summary>
    public virtual void ResetSilverValue()
    {
      Silver.ResetValue();
      DistributeValues();
    }
    
    /// <summary>
    ///  Resets the gold currency to 0.
    /// </summary>
    public virtual void ResetGoldValue()
    {
      Gold.ResetValue();
      DistributeValues();
    }
    
    /// <summary>
    ///  Resets the platinum currency to 0.
    /// </summary>
    public virtual void ResetPlatinumValue()
    {
      Platinum.ResetValue();
      DistributeValues();
    }
    
    /// <summary>
    ///  Resets all currencies to 0.
    /// </summary>
    public virtual void ResetAllValues()
    {
      Copper.ResetValue();
      Silver.ResetValue();
      Gold.ResetValue();
      Platinum.ResetValue();
    }
    
    /// <summary>
    ///  Distributes the values across all tiered currencies and converts excess values to the next tier up if necessary.
    /// </summary>
    public virtual void DistributeValues()
    {
        // Handle deficit in Copper by converting from Silver, Gold, and Platinum
        while (Copper.Value < 0 && Copper.IsTieredCurrency)
        {
            if (Silver.Value > 0)
            {
                Silver.SubtractValue(1);
                Copper.AddValue(Copper.Threshold);
            }
            else if (Gold.Value > 0)
            {
                Gold.SubtractValue(1);
                Silver.AddValue(Silver.Threshold - 1); // Convert 1 Gold to Silver, keeping 1 Silver to convert to Copper
                Copper.AddValue(Copper.Threshold);
            }
            else if (Platinum.Value > 0)
            {
                Platinum.SubtractValue(1);
                Gold.AddValue(Gold.Threshold - 1); // Convert 1 Platinum to Gold, keeping 1 Gold to convert further down
                Silver.AddValue(Silver.Threshold - 1); // Convert the Gold to Silver, keeping 1 Silver to convert to Copper
                Copper.AddValue(Copper.Threshold);
            }
            else
            {
                // If there's not enough in higher tiers, set Copper to 0
                Copper.Value = 0;
                break;
            }
        }

        // Handle deficit in Silver by converting from Gold and Platinum
        while (Silver.Value < 0 && Silver.IsTieredCurrency)
        {
            if (Gold.Value > 0)
            {
                Gold.SubtractValue(1);
                Silver.AddValue(Silver.Threshold);
            }
            else if (Platinum.Value > 0)
            {
                Platinum.SubtractValue(1);
                Gold.AddValue(Gold.Threshold - 1); // Convert 1 Platinum to Gold, keeping 1 Gold to convert to Silver
                Silver.AddValue(Silver.Threshold);
            }
            else
            {
                // If there's not enough in higher tiers, set Silver to 0
                Silver.Value = 0;
                break;
            }
        }

        // Handle deficit in Gold by converting from Platinum
        while (Gold.Value < 0 && Gold.IsTieredCurrency)
        {
            if (Platinum.Value > 0)
            {
                Platinum.SubtractValue(1);
                Gold.AddValue(Gold.Threshold);
            }
            else
            {
                // If there's not enough Platinum, set Gold to 0
                Gold.Value = 0;
                break;
            }
        }

        // Convert excess Copper to Silver
        while (Copper.Value >= Copper.Threshold && Copper.IsTieredCurrency)
        {
            Silver.AddValue(1);
            Copper.Value -= Copper.Threshold;
        }

        // Convert excess Silver to Gold
        while (Silver.Value >= Silver.Threshold && Silver.IsTieredCurrency)
        {
            Gold.AddValue(1);
            Silver.Value -= Silver.Threshold;
        }

        // Convert excess Gold to Platinum
        while (Gold.Value >= Gold.Threshold && Gold.IsTieredCurrency)
        {
            Platinum.AddValue(1);
            Gold.Value -= Gold.Threshold;
        }
    }
  }
}
