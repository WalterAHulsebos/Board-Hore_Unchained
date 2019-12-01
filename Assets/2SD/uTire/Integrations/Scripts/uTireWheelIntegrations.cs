using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSD
{
	[System.Serializable]
	public class WheelBase : System.Object
	{
		public Object m_wheelColliderBase { get; set; }
		public virtual Object wheelObject
		{
			get;
			set;
		}

		public Transform wheelTransform
		{
			get { throw new System.NotImplementedException(); }
		}
	}
	/// <summary>
	/// Acts as the built-in WheelCollider, it has the same functions
	/// It serves as a buffer between uTire Manager and the the wheel system used
	/// 
	/// </summary>
	public interface IWheel
	{
		/// <summary>
		/// The object we are using as wheel(~built in wheelCollider or something custom)
		/// </summary>
		[SerializeField]
		Object wheelObject { get; set; }

		Transform wheelTransform { get; }

		bool isGrounded { get; }

		float radius { get; set; }

		float suspensionDistance { get; }

	//	JointSpring suspensionSpring { get; }

		float steerAngle { get; set; }

		void GetWorldPose(out Vector3 pos, out Quaternion quat);

	//	bool GetGroundHit(out WheelHit hit);

		float targetDistance { get; }

		Vector3 GetGroundHitPoint();

		float springCompression { get; }

		Vector3 center { get; }
	}

#if TSD_INTEGRATION_WC3D
	/// <summary>
	/// Wheel Controller 3D
	/// </summary>
	public class WC3D : WheelBase, IWheel
	{
		[SerializeField]
		NWH.WheelController3D.WheelController wheelCollider;

		//Object m_wheelColliderBase;
	//	[SerializeField]
		public override Object wheelObject
		{
			get
			{
				return wheelCollider;
			}
			set
			{
				m_wheelColliderBase = value;
				wheelCollider = (NWH.WheelController3D.WheelController)value;
			}
		}

		public new Transform wheelTransform
		{
			get
			{
				return wheelCollider.transform;
			}
		}

		public float suspensionDistance
		{
			get { return wheelCollider.springLength; }
		}
		/*
		public JointSpring suspensionSpring
		{
			get { return wheelCollider.suspensionSpring; }
		}*/

		public float steerAngle
		{
			get { return wheelCollider.steerAngle; }
			set { wheelCollider.steerAngle = value; }
		}

		public void GetWorldPose(out Vector3 pos, out Quaternion quat)
		{
			wheelCollider.GetWorldPose(out pos, out quat);
		}

		public bool GetGroundHit(out NWH.WheelController3D.WheelController.WheelHit hit)
		{
			return wheelCollider.GetGroundHit(out hit);
		}

		public float radius
		{
			get
			{
				return wheelCollider.radius;
			}
			set
			{
				wheelCollider.radius = value;
			}
		}

		public bool isGrounded
		{
			get { return wheelCollider.isGrounded; }
		}

		public float targetDistance
		{
			get { return wheelCollider.springLength ; }
		}

		public Vector3 GetGroundHitPoint()
		{
			GetGroundHit(out TSD.uTireRuntime.uTireManager.WC3D_wHit);
			return TSD.uTireRuntime.uTireManager.WC3D_wHit.point; 
		}


		public float springCompression
		{
			get { return wheelCollider.springCompression; }
		}


		public Vector3 center
		{
			get { return Vector3.zero + new Vector3(0, -wheelCollider.springTravel, 0); }
		}
	}
#endif

	/// <summary>
	/// Standard WheelCollider
	/// </summary>
	[System.Serializable]
	public class WC : WheelBase, IWheel
	{
		[SerializeField]
		WheelCollider wheelCollider;

	//	Object m_wheelColliderBase;
	//	[SerializeField]
		public override Object wheelObject
		{
			get
			{
				return wheelCollider;
			}
			set
			{
				m_wheelColliderBase = value;
				wheelCollider = (WheelCollider)value;
			}
		}

		public new Transform wheelTransform
		{
			get
			{
				return wheelCollider.transform;
			}
		}

		public float suspensionDistance
		{
			get{return wheelCollider.suspensionDistance;}
		}

		public float steerAngle
		{
			get{return wheelCollider.steerAngle;}
			set{wheelCollider.steerAngle = value;}
		}

		public void GetWorldPose(out Vector3 pos, out Quaternion quat)
		{
			wheelCollider.GetWorldPose(out pos, out quat);
		}

		public bool GetGroundHit(out WheelHit hit)
		{
			return wheelCollider.GetGroundHit(out hit);
		}

		public float radius
		{
			get
			{
				return wheelCollider.radius;
			}
			set
			{
				wheelCollider.radius = value;
			}
		}

		public bool isGrounded
		{
			get { return wheelCollider.isGrounded; }
		}


		public float targetDistance
		{
			get { return wheelCollider.suspensionDistance; }
			//get { return wheelCollider.suspensionDistance * (1f- wheelCollider.suspensionSpring.targetPosition); }
		//	get { return wheelCollider.suspensionDistance * (1f - .5f); }
		}

		public Vector3 GetGroundHitPoint()
		{
			GetGroundHit(out TSD.uTireRuntime.uTireManager.wHit);
			return TSD.uTireRuntime.uTireManager.wHit.point; 
		}


		public float springCompression
		{
			get
			{
				float _springCompression = (-wheelTransform.InverseTransformPoint(GetGroundHitPoint()).y - radius);
				_springCompression = Mathf.Clamp(_springCompression, 0, targetDistance);
				_springCompression /= targetDistance + Mathf.Epsilon;

				return _springCompression;
			}
		}


		public Vector3 center
		{
			get { return wheelCollider.center; }
		}
	}
}