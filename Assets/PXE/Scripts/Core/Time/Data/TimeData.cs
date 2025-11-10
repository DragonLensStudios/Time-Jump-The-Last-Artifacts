#nullable enable
using UnityEngine;

namespace PXE.Core.Time.Data
{
    /// <summary>
    /// Represents the GameTimeData.
    /// The GameTimeData class provides functionality related to GameTimeData management.
    /// This class contains methods and properties that assist in managing and processing GameTimeData related tasks.
    /// </summary>
    [System.Serializable]
    public class TimeData
    {
        [field: SerializeField] public virtual float Year { get; set; } = 1;
        [field: SerializeField] public virtual float Month { get; set; } = 1;
        [field: SerializeField] public virtual float Week { get; set; } = 1;
        [field: SerializeField] public virtual float Day { get; set; } = 1;
        [field: SerializeField] public virtual float Hour { get; set; } = 1;
        [field: SerializeField] public virtual float Minute { get; set; }
        [field: SerializeField] public virtual float Second { get; set; }
        [field: SerializeField] public virtual float MilliSecond { get; set; }
        [field: SerializeField] public virtual float TimeScale { get; set; } = 1;

        /// <summary>
        /// Executes the GameTimeData method.
        /// Handles the GameTimeData functionality.
        /// </summary>
        public TimeData()
        {

        }

        public TimeData(float year = 1, float month = 1, float week = 1, float day = 1, float hour = 0, float minute = 0, float second = 0, float milliSecond = 0, float timeScale = 1)
        {
            Year = year;
            Month = month;
            Week = week;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            TimeScale = timeScale;
        }
        
        public TimeData(GameTimeObject gameTimeObject)
        {
            Year = gameTimeObject.Year;
            Month = gameTimeObject.Month;
            Week = gameTimeObject.Week;
            Day = gameTimeObject.Day;
            Hour = gameTimeObject.Hour;
            Minute = gameTimeObject.Minute;
            Second = gameTimeObject.Second;
            MilliSecond = gameTimeObject.MilliSecond;
            TimeScale = gameTimeObject.TimeScale;
        }
    }
}