namespace CommonGames.Utilities.CGTK
{
    using UnityEngine;
    
    using Object = UnityEngine.Object;
    
    public static partial class CGDebug
    {
        public static void Log(object message)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.Log(message);
            #endif
        }
    
        public static void Log(object message, Object context)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.Log(message, context);
            #endif
        }
    
        public static void LogFormat(string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogFormat(format, args);
            #endif
        }
    
        public static void LogFormat(Object context, string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogFormat(context, format, args);
            #endif
        }
    
        public static void LogAssertion(Object message)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogAssertion(message);
            #endif
        }
    
        public static void LogAssertion(Object message, Object context)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogAssertion(message, context);
            #endif
        }
    
        public static void LogAssertionFormat(string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogAssertionFormat(format, args);
            #endif
        }
    
        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogAssertionFormat(context, format, args);
            #endif
        }
    
        public static void LogError(object message)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogError(message);
            #endif
        }
    
        public static void LogError(object message, Object context)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogError(message, context);
            #endif
        }
    
        public static void LogErrorFormat(string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogErrorFormat(format, args);
            #endif
        }
    
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogErrorFormat(context, format, args);
            #endif
        }
    
        public static void LogWarning(object message)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogWarning(message);
            #endif
        }
    
        public static void LogWarning(object message, Object context)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogWarning(message, context);
            #endif
        }
    
        public static void LogWarningFormat(string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogWarningFormat(format, args);
            #endif
        }
    
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            #if UNITY_EDITOR || DEBUGBUILD
            Debug.LogWarningFormat(context, format, args);
            #endif
        }
    }
}