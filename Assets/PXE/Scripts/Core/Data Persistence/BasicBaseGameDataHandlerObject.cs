using PXE.Core.Data_Persistence.Data;
using UnityEngine;

namespace PXE.Core.Data_Persistence
{
    [CreateAssetMenu(fileName = "Basic Game Data Handler", menuName = "PXE/Data Persistence/Basic Game Data Handler")]
    public class BasicBaseGameDataHandlerObject : BaseGameDataHandlerObjectTyped<BaseGameData> 
    {
        public virtual void OnEnable()
        {
            DataHandler = new FileDataHandler();
            DataHandler.Initialize();
        }
    }
}