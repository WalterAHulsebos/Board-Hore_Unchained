using UnityEngine;
using Klak.Wiring;

namespace Fiftytwo.Wiring
{
    [AddComponentMenu("Klak/Wiring/★/Debug")]
    public class DebugLogOut : NodeBase
    {
#region Editable properties

        [SerializeField]
        string _message;

#endregion

#region Node I/O

        [Inlet]
        public void Log()
        {
            Debug.Log( _message );
        }

#endregion
    }
}
