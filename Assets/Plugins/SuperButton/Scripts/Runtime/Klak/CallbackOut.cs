using UnityEngine;
using Klak.Wiring;

namespace Fiftytwo
{
    [AddComponentMenu("Klak/Wiring/★/Callback Out")]
    public class CallbackOut : NodeBase
    {
        #region Editable properties

        [SerializeField]
        CallbackInput[] _callbackInputs;

        #endregion

        #region Node I/O

        [Inlet]
        public void Bang()
        {
            if( !enabled )
                return;
            foreach(var callbackInput in _callbackInputs)
                callbackInput.Callback();
        }
        #endregion
    }
}
