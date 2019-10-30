namespace CommonGames.Utilities.CGTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Object = UnityEngine.Object;
    using JetBrains.Annotations;

    public static partial class CGDebug
    {
        public class Gizmo
        {
            internal Action action { get; } 
            internal Color color { get; private set; } = UnityEngine.Color.red;
            internal Matrix4x4 matrix { get; private set; } = Matrix4x4.identity;
            internal float durationLeft = 0;

            internal Gizmo(Action action)
            {
                this.action = action;
            }

            public Gizmo Color(Color color)
            {
                this.color = color;
                return this;
            }

            public Gizmo Matrix(Matrix4x4 matrix)
            {
                this.matrix = matrix;
                return this;
            }
            
            public Gizmo Duration(float durationLeft = 1)
            {
                this.durationLeft = durationLeft;
                return this;
            }
        }

        /// <summary> Draw a solid box with position and size.</summary>
        [PublicAPI]
        public static Gizmo DrawCube(Vector3 position, Vector3 size = default)
            => Draw(new Gizmo(() => Gizmos.DrawCube(position, size)));

        /// <summary> Draw a wireframe box with position and size.</summary>
        [PublicAPI]
        public static Gizmo DrawWireCube(Vector3 position, Vector3 size = default)
            => Draw(new Gizmo(() => Gizmos.DrawWireCube(position, size)));

        /// <summary> Draw a camera frustum using the currently set Gizmos.matrix for it's location and rotation. </summary>
        [PublicAPI]
        public static Gizmo DrawFrustum(Camera camera)
            => Draw(new Gizmo(() => Gizmos.DrawFrustum(camera.transform.position, camera.fieldOfView,
                camera.farClipPlane, camera.nearClipPlane, camera.aspect)));

        /// <summary> Draw a camera frustum using the currently set Gizmos.matrix for it's location and rotation. </summary>
        [PublicAPI]
        public static Gizmo DrawFrustum(Vector3 position, float fieldOfView, float nearClipPlane, float farClipPlane,
            float aspect)
            => Draw(new Gizmo(() => Gizmos.DrawFrustum(position, fieldOfView, farClipPlane, nearClipPlane, aspect)));

        [PublicAPI]
        public static Gizmo DrawGUITexture(Rect screenRect,
            Texture texture,
            int leftBorder,
            int rightBorder,
            int topBorder,
            int bottomBorder,
            Material mat)
        {
            return Draw(new Gizmo(() =>
                Gizmos.DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat)));
        }

        [PublicAPI]
        public static Gizmo DrawIcon(Vector3 center, string name, bool allowScaling = true)
            => Draw(new Gizmo(() => Gizmos.DrawIcon(center, name, allowScaling)));

        [PublicAPI]
        public static Gizmo DrawRay(Vector3 from, Vector3 direction)
            => Draw(new Gizmo(() => Gizmos.DrawRay(from, direction)));

        [PublicAPI]
        public static Gizmo DrawRay(Ray ray)
            => Draw(new Gizmo(() => Gizmos.DrawRay(ray)));
        
        [PublicAPI]
        public static Gizmo DrawLine(Vector3 start, Vector3 end)
            => Draw(new Gizmo(() => Gizmos.DrawLine(start, end)));

        [PublicAPI]
        public static Gizmo DrawSphere(Vector3 center, float radius = 1f)
            => Draw(new Gizmo(() => Gizmos.DrawSphere(center, radius)));
        [PublicAPI]
        public static Gizmo DrawWireSphere(Vector3 center, float radius = default)
            => Draw(new Gizmo(() => Gizmos.DrawWireSphere(center, radius)));

        [PublicAPI]
        public static Gizmo DrawMesh(Mesh mesh,
            int submeshIndex = -1,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default) 
            => Draw(new Gizmo(() => Gizmos.DrawMesh(mesh, submeshIndex, position, rotation, scale)));
        
        //(Mesh mesh, Vector3 position = Vector3.zero, Quaternion rotation = Quaternion.identity, Vector3 scale = Vector3.one)
        
        [PublicAPI]
        public static Gizmo DrawWireMesh(Mesh mesh,
            int submeshIndex = -1,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
            => Draw(new Gizmo(() => Gizmos.DrawWireMesh(mesh, submeshIndex, position, rotation, scale)));
        
        /*
        public static Gizmo DrawWireMesh(Mesh mesh,
            int submeshIndex = 0,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
            => Draw(new Gizmo(() => Gizmos.DrawWireMesh(mesh, submeshIndex, position, rotation, scale)));
        */


        /*
        public static Gizmo[] DrawArrow(Vector3 origin, Vector3 direction)
        {
            //=> Draw(new Gizmo())
            float headSize = 0.1f;
            Vector3 end = origin + direction;
            Vector3 arrowBase = origin + direction * (1 - headSize);
            Vector3 left = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(left, direction).normalized;

            //Gizmos.DrawLine(origin, origin + direction);
            // 4 Arrowhead sides
            //Gizmos.DrawLine(end, arrowBase + left * headSize);
            //Gizmos.DrawLine(end, arrowBase + up * headSize);
            //Gizmos.DrawLine(end, arrowBase - left * headSize);
            //Gizmos.DrawLine(end, arrowBase - up * headSize);
            // 2 Arrowhed bases
            //Gizmos.DrawLine(arrowBase + left * headSize, arrowBase - left * headSize);
            //Gizmos.DrawLine(arrowBase + up * headSize, arrowBase - up * headSize);
            
            
            //TODO: EDIT
            return Draw
            (
                new Gizmo(() => Gizmos.DrawLine(origin, origin + direction)), 
                new Gizmo(() => Gizmos.DrawLine(end, arrowBase + left * headSize)),
                new Gizmo(() => Gizmos.DrawLine(end, arrowBase + up * headSize)),
                new Gizmo(() => Gizmos.DrawLine(end, arrowBase - left * headSize)),
                new Gizmo(() => Gizmos.DrawLine(end, arrowBase - up * headSize)),
                new Gizmo(() => Gizmos.DrawLine(arrowBase + left * headSize, arrowBase - left * headSize)),
                new Gizmo( () => Gizmos.DrawLine(arrowBase + up * headSize, arrowBase - up * headSize))
            );
        }
        */
        
        private static Gizmo Draw(Gizmo gizmo)
        {
            GizmoDrawer.Draw(gizmo);
            return gizmo;
        }
        
        private static Gizmo[] Draw(params Gizmo[] gizmos)
        {
            GizmoDrawer.Draw(gizmos);
            return gizmos;
        }

    }
}
