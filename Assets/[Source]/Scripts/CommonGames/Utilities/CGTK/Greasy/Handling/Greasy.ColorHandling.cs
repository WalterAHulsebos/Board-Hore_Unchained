using UnityEngine;

namespace CommonGames.Utilities.CGTK.Greasy
{
    public static partial class Greasy
    {
        public static Coroutine To(Color from, Color to, float duration, EaseType ease, Setter<Color> setter)
        {
            return CreateInterpolater(duration, ease, t => setter (Color.LerpUnclamped (from, to, t)));
        }
        public static Coroutine To(Color from, Color to, float duration, EaseMethod ease, Setter<Color> setter)
        {
            return CreateInterpolater(duration, ease, t => setter (Color.LerpUnclamped (from, to, t)));
        }
		
    }
}