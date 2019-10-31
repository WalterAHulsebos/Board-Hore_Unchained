using FSMHelper;
using RootMotion.Dynamics;
using System;
using UnityEngine;

public class Bail : MonoBehaviour
{
	[SerializeField]
	private Rigidbody _skaterRigidbody;

	[SerializeField]
	private CapsuleCollider _capsuleCollider;

	[SerializeField]
	private PuppetMaster _puppetMaster;

	public bool bailed;

	public Bail()
	{
	}

	private void CorrectRotation()
	{
		_puppetMaster.targetRoot.rotation = Quaternion.FromToRotation(_puppetMaster.targetRoot.up, Vector3.up) * _puppetMaster.targetRoot.rotation;
	}

	private void EnableCapsuleCollider()
	{
		_skaterRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		_capsuleCollider.enabled = true;
		_skaterRigidbody.isKinematic = false;
		_skaterRigidbody.useGravity = true;
	}

	public void OnBailed()
	{
		bailed = true;
		_puppetMaster.angularLimits = true;
		PlayerController.Instance.animationController.skaterAnim.CrossFade("Fall", 0.3f);
		PlayerController.Instance.playerSM.OnBailedSM();
		PlayerController.Instance.SetIKOnOff(0f);
	}

	public void RegainBalance()
	{
		CorrectRotation();
	}
}