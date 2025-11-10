using PXE.Core.Debug_Console.Scripts;
using UnityEngine;

namespace PXE.Core.Time.Commands
{
	public class TimeCommands
	{
		[ConsoleMethod( "time.scale", "Sets the Time.timeScale value" ), UnityEngine.Scripting.Preserve]
		public static void SetTimeScale( float value )
		{
			UnityEngine.Time.timeScale = Mathf.Max( value, 0f );
		}

		[ConsoleMethod( "time.scale", "Returns the current Time.timeScale value" ), UnityEngine.Scripting.Preserve]
		public static float GetTimeScale()
		{
			return UnityEngine.Time.timeScale;
		}
	}
}