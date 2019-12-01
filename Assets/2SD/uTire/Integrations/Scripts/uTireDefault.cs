using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSD
{
	namespace uTireIntegration
	{
		/// <summary>
		/// For consistency it's a separate class the rest in the namespace but it really is nothing but the base
		/// </summary>
		public class uTireDefault : uTireIntegrationBase
		{
			public static void LookForRequiredComponents()
			{
				//Safety check
				//if (vehicleRigidbody == null) { Debug.LogError("Drag in the vehicle's rigidbody first!"); return; }
			}
		}
	}
}