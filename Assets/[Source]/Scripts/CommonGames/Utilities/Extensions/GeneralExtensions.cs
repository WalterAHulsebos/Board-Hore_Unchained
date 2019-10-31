using System;
using System.Collections;
using System.Reflection;

using UnityEngine;
using UnityEngine.SceneManagement;

using JetBrains.Annotations;

using static CommonGames.Utilities.Extensions.Constants;

namespace CommonGames.Utilities.Extensions
{
    using SRandom = System.Random;

    public static partial class GeneralExtensions
    {
        /// <summary>
        /// Swaps values <paramref name="left"/> and <paramref name="right"/>
        /// </summary>
        public static void Swap<T>(ref T left, ref T right)
        {
            T __temp = right;
            
            right = left;
            left = __temp;
        }

        /// <summary>
        /// Do with target item.
        /// </summary>
        public static void Do<T>(this T t, params Action<T>[] actions) => actions.For(x => x(t));

        /// <summary>
        /// If bool is true, execute action.
        /// </summary>
        public static void If(this bool b, Action action)
        {
            if(b)
            {
                action();
            }
        }

        /// <summary>
        /// Executes the first action on true, the other on false.
        /// </summary>
        public static void IfElse(this bool b, Action onTrue, Action onFalse)
        {
            if(b)
            {
                onTrue();
            }
            else
            {
                onFalse();
            }
        }

        /// <summary>
        /// Execute target function i times where func returns true with overload i.
        /// </summary>
        public static void For(this int i, Func<int, bool> func, Action<int> action)
        {
            for(int j = 0; j < i; j++)
            {
                if(func(i))
                {
                    action(i);
                }
            }
        }

        /// <summary>
        /// Execute target function i times.
        /// </summary>
        public static void For(this int i, Action action)
        {
            for(int j = 0; j < i; j++)
            {
                action();
            }
        }

        /// <summary>
        /// Execute target function i times, with the index as overload.
        /// </summary>
        public static void For(this int i, Action<int> action)
        {
            for(int j = 0; j < i; j++)
            {
                action(j);
            }
        }

        /// <summary>
        /// Returns random value in range
        /// </summary>
        public static float RandomRange(float min, float max, SRandom random = null)
        {
            SRandom usedRandom = random ?? RANDOM;
            float lerp = (float)usedRandom.NextDouble();
            return Mathf.Lerp(min, max, lerp);
        }

        public static Vector3 RandomVector3(float min, float max, SRandom random = null)
        {
            SRandom usedRandom = random ?? RANDOM;
            return new Vector3(
                RandomRange(min, max, random), 
                RandomRange(min, max, random), 
                RandomRange(min, max, random));
        }

        public static Vector3Int RandomVector3Int(float min, float max, SRandom random = null)
        {
            Vector3 vector = RandomVector3(min, max, random);
            return new Vector3Int(
                Mathf.RoundToInt(vector.x), 
                Mathf.RoundToInt(vector.y), 
                Mathf.RoundToInt(vector.z));
        }

        /// <summary>
        /// Converts class to type T, instead of having to do (MyClass as T).
        /// </summary>
        public static T Convert<T>(this object obj) where T : class => obj as T;

        public static void Stop(this Coroutine coroutine, MonoBehaviour behaviour)
        {
            if(coroutine != null)
                behaviour.StopCoroutine(coroutine);
        }

        /// <summary>
        /// The next time a scene is loaded, execute this function.
        /// </summary>
        public static void AddInstanceToSceneLoaded(Action action)
        {
            void __Wrapper(Scene scene, LoadSceneMode mode)
            {
                action();
                SceneManager.sceneLoaded -= __Wrapper;
            }

            SceneManager.sceneLoaded += __Wrapper;
        }

        /// <summary>
        /// Shortcut for Unity's SetActive.
        /// </summary>
        public static void SetActive(this MonoBehaviour behaviour, bool enabled = true)
            => behaviour.gameObject.SetActive(enabled);

        /// <summary>
        /// Converts float into a string that visualizes the time digitally.
        /// </summary>
        public static string ToTimeString(this float seconds)
        {
            TimeSpan result = TimeSpan.FromSeconds(seconds);
            return string.Format($"{result.Hours}:{result.Minutes}:{result.Seconds}");
        }
    }

    public class ListArray<T>
    {
        public T[] Array { get; private set; }

        public int Length => Array.Length;
        public int Count { get; private set; }

        public T this[int index]
        {
            get => Array[index];
            set => Array[index] = value;
        }

        public ListArray(int length) => Array = new T[length];

        public void Add(T item)
        {
            Array[Length] = item;
            Count++;
        }

        public void Remove(T item, bool removeAll = false)
        {
            for (int i = 0; i < Length; i++)
                if(Array[i].Equals(item))
                {
                    RemoveAt(i);
                    if(!removeAll)
                        return;
                }
        }

        public void RemoveAt(int index)
        {
            int length = Length - index - 1;

            for (int i = 0; i < length; i++)
            {
                int current = index + length - i, next = current - 1;

                T item = Array[current];
                ref T nextItem = ref Array[next];

                Array[current] = nextItem;
                nextItem = item;
            }
        }

        public static implicit operator T[](ListArray<T> listArray) => listArray.Array;
        public static implicit operator ListArray<T>(T[] array) => new ListArray<T>(array.Length); 
    }

    /// <summary>
    /// An ragged array that decrements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FactoriumArray<T>
    {
        public T[] this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }

        public T this[int i, int j]
        {
            get => data[i][j];
            set => data[i][j] = value;
        }

        public int Length => data.Length;

        private T[][] data;

        public FactoriumArray(int length)
        {
            data = new T[length][];
            for (int i = 0; i < length; i++)
                data[i] = new T[length - i];
        }

        public void Reset()
        {
            int length = data.Length;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length - i; j++)
                    data[i][j] = default;
        }
    }

    /// <summary>
    /// A container for data that you usually use together when lerping.
    /// </summary>
    [Serializable]
    public struct Lerpable
    {
        /// <summary>
        /// Returns Mathf.lerp(min, max, curve.Evaluate(f));
        /// </summary>
        public float Get(in float f) => Mathf.Lerp(min, max, curve.Evaluate(f));

        [SerializeField]
        private float min, max;
        [SerializeField]
        private AnimationCurve curve;
    }

    /// <summary>
    /// Instead of having multiple booleans ie (talkingBlocksWalking, fallingBlocksWalking, etc) just use one lock and increment if something is
    /// blocking it and decrement it when something stops blocking it.
    /// </summary>
    public struct CGLock
    {
        private readonly int _value;
        
        [PublicAPI]
        public bool Locked => _value > 0;

        public CGLock(in int value = 0) => _value = value;
        
        public static CGLock operator +(in CGLock a, in CGLock b) => new CGLock(a._value + b._value);
        public static CGLock operator -(in CGLock a, in CGLock b) => new CGLock(a._value - b._value);
        
        public static CGLock operator ++(in CGLock a) => new CGLock(a._value + 1);
        public static CGLock operator --(in CGLock a) => new CGLock(a._value - 1);

        public static implicit operator int(in CGLock a) => a._value;
        public static implicit operator CGLock(in int i) => new CGLock(i);
        
        public static implicit operator bool(in CGLock cgLock) => !cgLock.Locked;
    }

    /// <summary>
    /// Used to invoke function with a return type by name.
    /// </summary>
    [Serializable]
    public class FuncEvent
    {
        public MonoBehaviour behaviour;
        public string name;

        private MethodInfo method = null;

        public T Invoke<T>()
        {
            if(method != null) return (T)method.Invoke(behaviour, null);
            
            Type type = behaviour.GetType();
            method = type.GetMethod(name);
            return (T)method.Invoke(behaviour, null);
        }
    }
}