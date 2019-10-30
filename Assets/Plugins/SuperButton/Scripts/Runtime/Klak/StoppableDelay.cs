using UnityEngine;
using System.Collections;
using Klak.Wiring;

namespace Fiftytwo.Wiring
{
    [AddComponentMenu("Klak/Wiring/★/Stoppable Delay")]
    public class StoppableDelay : NodeBase
    {
        #region Editable properties

        public enum TimeUnit { Second, Frame }

        [SerializeField]
        TimeUnit _timeUnit;

        [SerializeField]
        bool _random;
        [SerializeField]
        float _interval = 1;
        [SerializeField]
        float _interval2 = 2;

        #endregion

        #region Node I/O

        [Inlet]
        public void Play()
        {
            float interval;
            if( _random && _interval != _interval2 )
            {
                interval = _interval < _interval2
                    ? Random.Range( _interval, _interval2 )
                    : Random.Range( _interval2, _interval );
            }
            else
            {
                interval = _interval;
            }

            if( gameObject.activeInHierarchy )
                StartCoroutine( DoPlay( interval ) );
        }

        [Inlet]
        public void Stop()
        {
            StopAllCoroutines();
        }

        [SerializeField, Outlet]
        VoidEvent _outputEvent = new VoidEvent();

        #endregion

        #region MonoBehaviour functions

        IEnumerator DoPlay( float interval )
        {
            if( _timeUnit == TimeUnit.Second )
                yield return new WaitForSeconds( interval );
            else
            {
                for( int frameCounter = Mathf.RoundToInt( interval ); --frameCounter >= 0; )
                    yield return new WaitForEndOfFrame();
            }

            _outputEvent.Invoke();
        }

        #endregion
    }
}
