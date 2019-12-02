using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.CGTK;

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement
{
    //[ExecuteAlways]
    public class CenterOfMass : VehicleBehaviour
    {
        #region Variables

        [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0,-0.45f,0);
        [SerializeField] private bool showDebug = true;
     
        private Vector3 _prevOffset = Vector3.zero;
        private Vector3 _centerOfMass;
        
        #endregion

        #region Methods

        protected override void Start()
        {
            base.Start();
            
            _centerOfMass = _vehicle.rigidbody.centerOfMass;
        }

        //It's not certain if the _vehicle has been initialized at this point in time so I scapped this.
        /*
        private void OnValidate()
        {
            if(centerOfMassOffset != _prevOffset)
            {
                _vehicle.rigidbody.centerOfMass = _centerOfMass + centerOfMassOffset;
            }
            
            _prevOffset = centerOfMassOffset;
        }
        */

        private void Update()
        {
            if(centerOfMassOffset != _prevOffset)
            {
                _vehicle.rigidbody.centerOfMass = _centerOfMass + centerOfMassOffset;
            }
            
            _prevOffset = centerOfMassOffset;
            
            if(!showDebug) return;
            
            //Radius is .1 units..
            float __radius = 0.1f;
            //Except if there's a MeshFilter component on this object,
            //then we'll set the radius to 10% of the bound-size.
            if(TryGetComponent(component: out MeshFilter __meshFilter))
            {
                __radius = __meshFilter.sharedMesh.bounds.size.z / 10f;
            }
                
            CGDebug.DrawSphere(
                center: _vehicle.rigidbody.transform.TransformPoint(position: _vehicle.rigidbody.centerOfMass), 
                radius: __radius).Color(Color.green);
        }
        
        #endregion
    }
}