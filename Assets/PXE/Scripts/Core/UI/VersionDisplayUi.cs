using PXE.Core.Objects;
using TMPro;
using UnityEngine;

namespace PXE.Core.UI
{
    public class VersionDisplayUi : ObjectController
    {
        [field: SerializeField] public TMP_Text versionText;

        public override void Awake()
        {
            base.Awake();
            if (versionText == null)
            {
                versionText = GetComponent<TMP_Text>();
            }

            versionText.text = $"Version: {Application.version}";
        }
    }
}
