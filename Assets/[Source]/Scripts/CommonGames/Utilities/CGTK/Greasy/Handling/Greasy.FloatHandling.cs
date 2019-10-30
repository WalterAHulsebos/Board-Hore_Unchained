using UnityEngine;

namespace CommonGames.Utilities.CGTK.Greasy
{
    public static partial class Greasy
    {
        public static Coroutine To(float from, float to, float duration, EaseType ease, Setter<float> setter)
        {
            return CreateInterpolater (duration, ease, t => setter (Mathf.LerpUnclamped (from, to, t)));
        }
        public static Coroutine To(float from, float to, float duration, EaseMethod ease, Setter<float> setter)
        {
            return CreateInterpolater (duration, ease, t => setter (Mathf.LerpUnclamped (from, to, t)));
        }
		
    }
}
