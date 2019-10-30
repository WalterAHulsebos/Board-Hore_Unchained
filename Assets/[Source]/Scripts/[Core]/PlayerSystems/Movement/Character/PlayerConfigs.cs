using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KinematicCharacterController;

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;

using Sirenix.OdinInspector;

using static Core.PlayerSystems.Movement.PlayerController;

using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

#if Odin_Inspector
using ScriptableObject = Sirenix.OdinInspector.SerializedScriptableObject;
#endif

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement
{
	[CreateAssetMenu()]
	public class PlayerConfigs : ScriptableObject
	{
		#region Variables

		#region Serialized
		
		[FoldoutGroup("Ground Movement")]
		[SerializeField] public float maxGroundMoveSpeed = 10f;
		[FoldoutGroup("Ground Movement")]
		[SerializeField] public float groundMovementSharpness = 100f;
		[FoldoutGroup("Ground Movement")]
		[SerializeField] public float orientationSharpness = 10f;
		[FoldoutGroup("Ground Movement")]
		[SerializeField] public float orientationTime = .5f;
		

		[FoldoutGroup("Air Movement")]
		[SerializeField] public float 
			maxAirMoveSpeed = 10f, 
			airAccelerationSpeed = 4f, 
			drag = 0.2f;

		[FoldoutGroup("Jumping")]
		[SerializeField] public bool allowJumpingWhenSliding = true;
		[FoldoutGroup("Jumping")]
		[SerializeField] public float 
			jumpUpSpeed = 10f, 
			jumpScalableForwardSpeed = 3f,
			jumpPreGroundingGraceTime = 0f, 
			jumpPostGroundingGraceTime = 0f;

		[FoldoutGroup("Misc")]
		[SerializeField] public List<Collider> ignoredColliders = new List<Collider>();
		[FoldoutGroup("Misc")]
		[SerializeField] public bool orientTowardsGravity = true;
		[FoldoutGroup("Misc")]
		[SerializeField] public Vector3 gravity = new Vector3(0, -30f, 0);
		//[FoldoutGroup("Misc")]
		//[SerializeField] public Transform meshRoot;
		
		#endregion

		#endregion
	}
}
