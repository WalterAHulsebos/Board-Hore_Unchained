using UnityEngine;
using Klak.Wiring;
using Klak.Motion;

namespace Fiftytwo
{
    [AddComponentMenu("Klak/Wiring/★/SmoothFollow Out")]
    public class SmoothFollowOut : NodeBase
    {
        #region Editable properties

        [SerializeField]
        SmoothFollow _smoothFollow;

        [SerializeField]
        Transform _normal;
        
        [SerializeField]
        Transform _over;

        [SerializeField]
        Transform _overStatic;

        [SerializeField]
        Transform _pressed;

        [SerializeField]
        Transform _drag;

        #endregion

        #region Node I/O

        [Inlet]
        public void Normal ()
        {
            if( !enabled )
                return;
            _smoothFollow.target = _normal;
        }

        [Inlet]
        public void Over ()
        {
            if( !enabled )
                return;
            _smoothFollow.target = _over;
        }

        [Inlet]
        public void OverStatic ()
        {
            if( !enabled )
                return;
            _smoothFollow.target = _overStatic;
        }

        [Inlet]
        public void Pressed ()
        {
            if( !enabled )
                return;
            _smoothFollow.target = _pressed;
        }

        [Inlet]
        public void Drag ()
        {
            if( !enabled )
                return;
            _smoothFollow.target = _drag;
        }

        #endregion
    }
}