using PXE.Core.Objects;
using UnityEngine;

namespace PXE._TESTING
{
    public class MyTesterManager : ObjectController
    {
        [field: SerializeField] public string MyTestString { get; set; }
        [field: SerializeField] public int MyTestInt { get; set; }
        [field: SerializeField] public float MyTestFloat { get; set; }
        [field: SerializeField] public bool MyTestBool { get; set; }
        [field: SerializeField] public Vector3 MyTestVector3 { get; set; }
        [field: SerializeField] public GameObject MyTestGameObject { get; set; }
    }
}