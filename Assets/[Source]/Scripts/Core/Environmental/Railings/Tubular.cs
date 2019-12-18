using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Environmental.Railings;

using CommonGames.Utilities.Extensions;

namespace Core.Environmental.Railings 
{
	using static CommonGames.Utilities.Defaults;
	//using Curve = Core.Environmental.Railings.Curve;
	
	public class Tubular
	{
		public static Mesh Build(Curve curve, int tubularSegments, float radius, int radialSegments, bool closed) 
		{
			List<Vector3> __vertices = new List<Vector3>();
			List<Vector3> __normals = new List<Vector3>();
			List<Vector4> __tangents = new List<Vector4>();
			List<Vector2> __uvs = new List<Vector2>();
			List<int> __indices = new List<int>();

			List<FrenetFrame> __frames = curve.ComputeFrenetFrames(segments: tubularSegments, closed: closed);

			for(int __i = 0; __i < tubularSegments; __i++) 
			{
				GenerateSegment(curve, __frames, tubularSegments, radius, radialSegments, __vertices, __normals, __tangents, __i);
			}
			GenerateSegment(curve, __frames, tubularSegments, radius, radialSegments, __vertices, __normals, __tangents, i: (!closed)? tubularSegments : 0);

			for (int __i = 0; __i <= tubularSegments; __i++) 
			{
				for (int __j = 0; __j <= radialSegments; __j++) 
				{
					float __u = 1f * __j / radialSegments;
					float __v = 1f * __i / tubularSegments;
					__uvs.Add(new Vector2(__u, __v));
				}
			}

			for (int __j = 1; __j <= tubularSegments; __j++) 
			{
				for (int __i = 1; __i <= radialSegments; __i++) 
				{
					int __a = (radialSegments + 1) * (__j - 1) + (__i - 1);
					int __b = (radialSegments + 1) * __j + (__i - 1);
					int __c = (radialSegments + 1) * __j + __i;
					int __d = (radialSegments + 1) * (__j - 1) + __i;

					// faces
					__indices.Add(__a); __indices.Add(__d); __indices.Add(__b);
					__indices.Add(__b); __indices.Add(__d); __indices.Add(__c);
				}
			}

			Mesh __mesh = new Mesh
			{
				vertices = __vertices.ToArray(),
				normals = __normals.ToArray(),
				tangents = __tangents.ToArray(),
				uv = __uvs.ToArray()
			};
			
			__mesh.SetIndices(__indices.ToArray(), MeshTopology.Triangles, 0);
			return __mesh;
		}

		private static void GenerateSegment
		(
			Curve curve, 
			IReadOnlyList<FrenetFrame> frames, 
			int tubularSegments, 
			float radius,
			int radialSegments,
			ICollection<Vector3> vertices, 
			ICollection<Vector3> normals, 
			ICollection<Vector4> tangents, 
			int i
		)
		{
			float __u = 1f * i / tubularSegments;
			Vector3 __p = curve.GetPointAt(u: __u);
			FrenetFrame __fr = frames[index: i];

			Vector3 __n = __fr.Normal;
			Vector3 __b = __fr.Binormal;

			for(int __j = 0; __j <= radialSegments; __j++)
			{
				float __v = 1f * __j / radialSegments * PI_2;
				float __sin = Mathf.Sin(f: __v);
				float __cos = Mathf.Cos(f: __v);

				Vector3 __normal = (__cos * __n + __sin * __b).normalized;
				vertices.Add(item: __p + radius * __normal);
				normals.Add(item: __normal);

				Vector3 __tangent = __fr.Tangent;
				tangents.Add(item: new Vector4(x: __tangent.x, y: __tangent.y, z: __tangent.z, 0f));
			}
		}

	}
}