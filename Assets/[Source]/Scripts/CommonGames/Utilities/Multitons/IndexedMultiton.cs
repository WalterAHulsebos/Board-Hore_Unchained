using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Utilities.Extensions;

namespace CommonGames.Utilities
{
    /// <summary> A Multiton in which you can set the index in the list yourself. </summary>
    public class IndexedMultiton<T> : Multiton<T> where T : Multiton<T>
    {
        //private int _index = 0;

        //[BoxGroup("IndexBox", showLabel: false)] [HorizontalGroup("IndexBox/Index")]
        //[OdinSerialize]
        public int Index; // { get; set; }
        
        /*
        {
            get => GetIndexFromInstances;
            set => _index = value;
        }
        */

        //[HorizontalGroup("IndexBox/Index", 0.01f)]
        //[HideLabel]
        //[OdinSerialize] public bool lockIndex = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            Index = GetIndexFromInstance;
        }

        private void Reset()
            => Index = GetIndexFromInstance;

        private void OnValidate()
            => Index = GetIndexFromInstance;

        private int GetIndexFromInstance => IndexFromInstance(this);

        //private int GetIndexFromInstances => Instances.GetIndex(checkInstance => checkInstance == this);
    }
}