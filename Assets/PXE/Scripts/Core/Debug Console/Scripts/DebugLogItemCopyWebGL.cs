#if !UNITY_EDITOR && UNITY_WEBGL

using System.Runtime.InteropServices;
using PXE.Core.Debug_Console.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PXE.Core.Debug_Console
{
	public class DebugLogItemCopyWebGL : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[DllImport( "__Internal" )]
		private static extern void IngameDebugConsoleStartCopy( string textToCopy );
		[DllImport( "__Internal" )]
		private static extern void IngameDebugConsoleCancelCopy();

		private DebugLogItem logItem;

		public void Initialize( DebugLogItem logItem )
		{
			this.logItem = logItem;
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			string log = logItem.GetCopyContent();
			if( !string.IsNullOrEmpty( log ) )
				IngameDebugConsoleStartCopy( log );
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			if( eventData.dragging )
				IngameDebugConsoleCancelCopy();
		}
	}
}
#endif