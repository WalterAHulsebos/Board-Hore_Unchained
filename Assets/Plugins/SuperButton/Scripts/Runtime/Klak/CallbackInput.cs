using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using Klak.Wiring;

namespace Fiftytwo
{
    [AddComponentMenu("Klak/Wiring/★/Callback Input")]
    public class CallbackInput : NodeBase
    {
        #region Node I/O

        [SerializeField, Outlet]
        VoidEvent _onCallbackEvent = new VoidEvent();

        #endregion

        #region MonoBehaviour functions

        public void Callback()
        {
            _onCallbackEvent.Invoke();
        }

        #endregion
    }
}
