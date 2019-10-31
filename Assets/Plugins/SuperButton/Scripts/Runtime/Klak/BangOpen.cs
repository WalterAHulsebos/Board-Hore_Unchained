using UnityEngine;
using Klak.Wiring;

namespace Fiftytwo
{
    [AddComponentMenu("Klak/Wiring/★/Bang Open")]
    public class BangOpen : NodeBase
    {
        #region Editable properties

        [SerializeField]
        BangFilter[] _bangInputs;

        #endregion

        #region Node I/O

        [Inlet]
        public void Open()
        {
            if( !enabled )
                return;
            foreach(var bangInput in _bangInputs)
                bangInput.Open();
        }
        #endregion
    }
}
