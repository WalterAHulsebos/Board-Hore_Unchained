using UnityEngine;
using Klak.Math;

namespace Klak.Wiring
{
    [AddComponentMenu("Klak/Wiring/★/Stoppable Float Filter")]
    public class StoppableFloatFilter : NodeBase
    {
        #region Editable properties

        [SerializeField]
        AnimationCurve _responseCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        FloatInterpolator.Config _interpolator = FloatInterpolator.Config.Direct;

        [SerializeField]
        float _amplitude = 1.0f;

        [SerializeField]
        float _bias = 0.0f;

        [SerializeField]
        bool _startAtStart = true; 

        #endregion

        #region Node I/O

        [Inlet]
        public float input {
            set {
                if (!enabled) return;

                _inputValue = value;

                if (_interpolator.enabled)
                    _floatValue.targetValue = EvalResponse();
                else
                    _outputEvent.Invoke(EvalResponse());
            }
        }

        [Inlet]
        public float interSpeed {
            set {
                if (!enabled) return;

                _interSpeedValue = value;

                if (_interpolator.enabled)
                    _interpolator.interpolationSpeed = _interSpeedValue;
            }
        }

        [Inlet]
        public void Play()
        {
            _isPlaying = true;
        }

        [Inlet]
        public void Stop()
        {
            _isPlaying = false;
        }

        [SerializeField, Outlet]
        FloatEvent _outputEvent = new FloatEvent();

        #endregion

        #region Private members

        float _inputValue;
        float _interSpeedValue;
        FloatInterpolator _floatValue;
        bool _isPlaying;

        float EvalResponse()
        {
            return _responseCurve.Evaluate(_inputValue) * _amplitude + _bias;
        }

        #endregion

        #region MonoBehaviour functions

        void Start()
        {
            _floatValue = new FloatInterpolator(EvalResponse(), _interpolator);

            if( _startAtStart )
                Play();
        }

        void Update()
        {
            if ( _isPlaying && _interpolator.enabled)
                _outputEvent.Invoke(_floatValue.Step());
        }

        #endregion
    }
}
