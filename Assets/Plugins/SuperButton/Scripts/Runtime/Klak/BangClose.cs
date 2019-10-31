using UnityEngine;
using Klak.Wiring;

namespace Fiftytwo
{
    [AddComponentMenu("Klak/Wiring/★/Bang Close")]
    public class BangClose : NodeBase
    {
        #region Editable properties

        [SerializeField]
        BangFilter[] _bangInputs;

        #endregion

        #region Node I/O

        [Inlet]
        public void Close()
        {
            if( !enabled )
                return;
            foreach(var bangInput in _bangInputs)
                bangInput.Close();
        }
        #endregion
    }
}
