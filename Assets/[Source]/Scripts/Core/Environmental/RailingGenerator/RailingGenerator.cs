using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.CGTK;

using PathCreation;
using PathCreation.Utility;
using PathCreation.Examples;

using Sirenix.OdinInspector;

namespace Core.RailingGenerator
{
    [ExecuteInEditMode]
    public class RailingGenerator : PathSceneTool
    {
        //TODO: Walter - Replace Instantiation with pooled Spawning.
        
        [SerializeField] private float radius = .5f;
        [SerializeField] private int radialSegments = 24;
        [SerializeField] private Material material;

        [Space]
        
        [SerializeField] private GameObject parent;
        
        [MinValue(_MIN_SPACING)]
        [SerializeField] private float loopSpacing = 4;

        
        //TODO: CALCULATE THIS..
        private float _segmentCount = 100;

        private const float _MIN_SPACING = .1f;
        
        private const float _PI2 = Mathf.PI * 2f;

        [Button]
        private void Generate()
        {
            if (pathCreator == null) return;
            
            //DestroyOldObjects();

            VertexPath __path = pathCreator.path;

            loopSpacing = Mathf.Max(_MIN_SPACING, loopSpacing);
            float __dst = 0;
            
            //GenerateTube();

            while (__dst < __path.length)
            {
                Vector3 __pos = __path.GetPointAtDistance(__dst);
                Quaternion __rot = __path.GetRotationAtDistance(__dst);
                
                //__GenerateRing(position: __pos, rotation: __rot);
                __dst += loopSpacing;
            }

            void __GenerateRing(in Vector3 position, in Quaternion rotation)
            {
                float __vStep = (2f * Mathf.PI) / _segmentCount;
		
                for (int __segmentStep = 0; __segmentStep < _segmentCount; __segmentStep++)
                {
                    //Vector3 __point = GetPointOnTorus(0f, __segmentStep * __vStep);
                    //CGDebug.DrawSphere(__point, 0.1f);
                }
            }
        }

        private void DestroyOldObjects()
        {
            int __numChildren = parent.transform.childCount;
            for (int __i = __numChildren - 1; __i >= 0; __i--)
            {
                DestroyImmediate(parent.transform.GetChild (__i).gameObject, false);
            }
        }

        protected override void PathUpdated()
        {
	        Generate();
        }
        
    }
}