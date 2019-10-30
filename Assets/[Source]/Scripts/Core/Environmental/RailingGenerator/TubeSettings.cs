using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Curve;

using Sirenix.OdinInspector;

namespace Tubular 
{
	[RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer))]
	public class TubeSettings : MonoBehaviour
	{
		[SerializeField] private TubeGenerator _tubeGenerator = null;
		[SerializeField] private MeshFilter _meshFilter = null;

		[SerializeField] private MeshCollider _meshCollider = null;

		public const int TUBE_SEGMENTS_DEFAULT = 100;
		
		public int tubeSegments = TUBE_SEGMENTS_DEFAULT;
		[SerializeField] private float radius = 0.2f;
		[SerializeField] private int radialSegments = 32;
		[SerializeField] private bool closed = false;

		private void OnEnable()
		{
			_tubeGenerator.refresh += Bake;
		}
		private void OnDisable()
		{
			_tubeGenerator.refresh -= Bake;
		}

		private void OnValidate()
		{
			_tubeGenerator.refresh -= Bake;
			_tubeGenerator.refresh += Bake;
			
			_tubeGenerator = _tubeGenerator ? _tubeGenerator : GetComponent<TubeGenerator>();
			_meshFilter = _meshFilter ? _meshFilter : GetComponent<MeshFilter>();
			_meshCollider = _meshCollider ? _meshCollider : GetComponent<MeshCollider>();
			
			Bake();
		}

		private void Reset()
		{
			_tubeGenerator.refresh -= Bake;
			_tubeGenerator.refresh += Bake;
			
			_tubeGenerator = _tubeGenerator ? _tubeGenerator : GetComponent<TubeGenerator>();
			_meshFilter = _meshFilter ? _meshFilter : GetComponent<MeshFilter>();
			_meshCollider = _meshCollider ? _meshCollider : GetComponent<MeshCollider>();
			
			Bake();
		}

		private void Start()
		{
			_tubeGenerator = _tubeGenerator ? _tubeGenerator : GetComponent<TubeGenerator>();
			_meshFilter = _meshFilter ? _meshFilter : GetComponent<MeshFilter>();
			_meshCollider = _meshCollider ? _meshCollider : GetComponent<MeshCollider>();
		}

		[Button]
		private void Bake() 
		{
			Curve.Curve __curve = _tubeGenerator.Build();

			Mesh __mesh = Tubular.Build(__curve, tubeSegments, radius, radialSegments, closed);

			_meshFilter.sharedMesh = __mesh;
			_meshCollider.sharedMesh = __mesh;

			/*
			
			if(_trigger == null) return;
			
			Mesh __triggerMesh = Tubular.Build(__curve, tubeSegments, radius * 3f, radialSegments, closed);
				
			_trigger.sharedMesh = __mesh;
			_trigger.isTrigger = true;
			
			*/
		}

	}
	
}