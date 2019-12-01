using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSD.uTireRuntime
{
	public class uTireWorldSpaceSynchronizer : MonoBehaviour
	{
		[HideInInspector, Tooltip("The settings of this behavior will be copied")]
		public uTireWorldSpaceBehaviour templateBehavior;
		
		public List<uTireWorldSpaceBehaviour> uTireWorldSpace = new List<uTireWorldSpaceBehaviour>();
		
		// Use this for initialization
		void Start()
		{
			synchronize();
		}

		[ContextMenu("Synchronize uTireWorldSpaceBehaviour scripts")]
		public void synchronize()
		{
			if (templateBehavior == null)
			{
				Debug.LogWarning("(uTireWorldSpaceSynchronizer)No templateBehavior was asigned.", this);
				return;
			}
			if (uTireWorldSpace.Count == 0)
			{
				uTireWorldSpace = GetComponentsInChildren<uTireWorldSpaceBehaviour>().Where(utire => utire != templateBehavior).ToList();
			}
			foreach (var item in uTireWorldSpace)
			{
				item.pasteComponentData(templateBehavior);
			}
		}
	}
}