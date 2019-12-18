using System.Collections;
using System.Collections.Generic;
using CommonGames.Utilities.Extensions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Core.PlayerSystems.Movement.Abilities;

namespace Core.Environmental.Railings
{

    [RequireComponent(typeof(TubeGenerator), typeof(TubeSettings))]
    public class GrindBar : MonoBehaviour
    {
        [SerializeField] private TubeGenerator tubeGenerator = null;

        public void Initialize()
            => tubeGenerator = tubeGenerator ? tubeGenerator : GetComponent<TubeGenerator>();

        private void Start()
            => Initialize();            

        private void OnCollisionEnter(Collision collision)
        {
            GrindAbility __grindAbility = collision.transform.root.GetComponentInChildren<GrindAbility>();

            if(!__grindAbility) return;

            __grindAbility.AttachToBar(bar: tubeGenerator);
        }
        
        #if UNITY_EDITOR
        [MenuItem("GameObject/3D Object/Create GrindBar")]
        #endif
        private static void CreateGrindBar()
        {
            GameObject __gameObject = new GameObject("New GrindBar");

            GrindBar __grindBar = __gameObject.AddComponent<GrindBar>();

            TubeSettings __tubeSettings = __grindBar.GetComponent<TubeSettings>();
            
            __tubeSettings.Initialize();

            //Vector3 __forward = __gameObject.transform.forward;

            //__grindBar.tubeGenerator.AddPoint(__forward);
            //__grindBar.tubeGenerator.AddPoint(-__forward);
            
            //__grindBar.tubeGenerator.Points.AddRange(new []{__forward, -__forward});
            
            //__grindBar.tubeGenerator.Points.Add(__forward);
            //__grindBar.tubeGenerator.Points.Add(-__forward);
        }
    }
}