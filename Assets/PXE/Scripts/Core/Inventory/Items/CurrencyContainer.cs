using System;
using Newtonsoft.Json;
using PXE.Core.Enums;
using PXE.Core.Inventory.Interfaces;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    /// <summary>
    ///  This class is used to represent a currency in the game.
    /// </summary>
    [Serializable]
    public class CurrencyContainer : ICurrencyContainer
    {

        [field: Tooltip("The type of this currency.")]
        [field: SerializeField] public virtual CurrencyType Type { get; set; }
        
        [Tooltip("The value of this currency.")]
        [SerializeField] protected long _value;
        public virtual long Value
        {
            get => _value;
            set
            {
                if (HasLimit)
                {
                    _value = value > Limit ? Limit : value;
                }
                else
                {
                    _value = value;
                }
            }
        }
        
        [field: Tooltip("Whether or not this currency is a tiered currency.")]
        [field: SerializeField] public virtual bool IsTieredCurrency { get; set; }

        [field: Tooltip("The threshold for this currency to become a tiered currency.")]
        [field: SerializeField] public virtual long Threshold { get; set; }
        
        [field: Tooltip("If there is a limit to this currency.")]
        [field: SerializeField] public virtual bool HasLimit { get; set; }
        
        [field: Tooltip("The limit of this currency.")]
        [field: SerializeField] public virtual long Limit { get; set; }
        
        [field: Tooltip("The icon of the currency.")]
        [field: SerializeField, JsonIgnore] 
        [JsonIgnore] public virtual Sprite CurrencyIcon { get; set; }

        /// <summary>
        ///   Adds the given value to the currency's value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void AddValue(long value)
        {
            Value += value;
        }
        
        /// <summary>
        ///  Subtracts the given value from the currency's value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SubtractValue(long value)
        {
            Value -= value;
        }
        
        /// <summary>
        ///  Multiplies the currency's value by the given value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void MultiplyValue(long value)
        {
            Value *= value;
        }
        
        /// <summary>
        ///  Divides the currency's value by the given value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void DivideValue(long value)
        {
            if (value != 0)
            {
                Value /= value;
            }
            else
            {
                Debug.LogError("Cannot divide by 0.");
            }
        }
        
        /// <summary>
        /// Sets the currency's value to the given value and distributes it across all tiered currencies.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public virtual void SetValue(long value)
        {
            Value = value;
        }
        
        /// <summary>
        ///  Resets the currency's value to 0.
        /// </summary>
        public virtual void ResetValue()
        {
            Value = 0;
        }
        

    } 
}

