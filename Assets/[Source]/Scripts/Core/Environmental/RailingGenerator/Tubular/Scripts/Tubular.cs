using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Curve;

namespace Tubular 
{
	public class Tubular 
	{
		private const float _PI2 = Mathf.PI * 2f;

		public static Mesh Build(Curve.Curve curve, int tubularSegments, float radius, int radialSegments, bool closed) 
		{
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector4> tangents = new List<Vector4>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> indices = new List<int>();

			List<FrenetFrame> frames = curve.ComputeFrenetFrames(tubularSegments, closed);

			for(int i = 0; i < tubularSegments; i++) {
				GenerateSegment(curve, frames, tubularSegments, radius, radialSegments, vertices, normals, tangents, i);
			}
			GenerateSegment(curve, frames, tubularSegments, radius, radialSegments, vertices, normals, tangents, (!closed) ? tubularSegments : 0);

			for (int i = 0; i <= tubularSegments; i++) 
			{
				for (int j = 0; j <= radialSegments; j++) 
				{
					float u = 1f * j / radialSegments;
					float v = 1f * i / tubularSegments;
					uvs.Add(new Vector2(u, v));
				}
			}

			for (int j = 1; j <= tubularSegments; j++) 
			{
				for (int i = 1; i <= radialSegments; i++) 
				{
					int a = (radialSegments + 1) * (j - 1) + (i - 1);
					int b = (radialSegments + 1) * j + (i - 1);
					int c = (radialSegments + 1) * j + i;
					int d = (radialSegments + 1) * (j - 1) + i;

					// faces
					indices.Add(a); indices.Add(d); indices.Add(b);
					indices.Add(b); indices.Add(d); indices.Add(c);
				}
			}

			Mesh __mesh = new Mesh
			{
				vertices = vertices.ToArray(),
				normals = normals.ToArray(),
				tangents = tangents.ToArray(),
				uv = uvs.ToArray()
			};
			
			__mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
			return __mesh;
		}

		static void GenerateSegment(
			Curve.Curve curve, 
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
			float u = 1f * i / tubularSegments;
			Vector3 p = curve.GetPointAt(u);
			FrenetFrame fr = frames[i];

			Vector3 N = fr.Normal;
			Vector3 B = fr.Binormal;

			for(int j = 0; j <= radialSegments; j++) {
				float v = 1f * j / radialSegments * _PI2;
				float sin = Mathf.Sin(v);
				float cos = Mathf.Cos(v);

				Vector3 normal = (cos * N + sin * B).normalized;
				vertices.Add(p + radius * normal);
				normals.Add(normal);

				Vector3 tangent = fr.Tangent;
				tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
			}
		}

	}

}

