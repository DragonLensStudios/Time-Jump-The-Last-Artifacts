using PXE.Core.Data_Persistence;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Data_Persistence.Data
{
    [CreateAssetMenu(fileName = "Oceans Call Game Data Handler", menuName = "PXE/Data Persistence/Oceans Call Game Data Handler")]
    public class OceansCallDataHandlerObject : BaseGameDataHandlerObjectTyped<OceansCallGameData>
    {
        public void OnEnable()
        {
            DataHandler = new FileDataHandler();
            DataHandler.Initialize();
        }
    }
}