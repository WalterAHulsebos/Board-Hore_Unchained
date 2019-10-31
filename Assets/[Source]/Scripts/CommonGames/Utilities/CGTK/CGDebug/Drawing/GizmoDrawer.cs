using Sirenix.Utilities;
using CommonGames.Utilities.Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using CommonGames.Utilities;
using static CommonGames.Utilities.CGTK.CGDebug;

using JetBrains.Annotations;

namespace CommonGames.Utilities.CGTK
{
    
    public sealed class GizmoDrawer : Singleton<GizmoDrawer>
    {
        private readonly List<Gizmo> gizmos = new List<CGDebug.Gizmo>();
    
        private void Update()
            => gizmos.For(gizmo => gizmo.durationLeft -= Time.deltaTime);
    
        private void OnDrawGizmos()
        {
            foreach (Gizmo _gizmo in gizmos)
            {
                Color _prevColor = Gizmos.color;
                Matrix4x4 _prevMatrix = Gizmos.matrix;
                Gizmos.color = _gizmo.color;
    
                if (_gizmo.matrix != default)
                {
                    Gizmos.matrix = _gizmo.matrix;
                }
                _gizmo.action();
    
                Gizmos.color = _prevColor;
                Gizmos.matrix = _prevMatrix;
            }
            
            gizmos.CGRemoveAll(gizmo => gizmo.durationLeft <= 0);
        }
    
        [PublicAPI]
        public static void Draw(in Gizmo drawing)
            => Instance.gizmos.Add(drawing);
        
        [PublicAPI]
        public static void Draw(params Gizmo[] drawings)
            => Instance.gizmos.AddRange(drawings);
    }
}