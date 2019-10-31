using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.PlayerSystems.Movement.Abilities;

[RequireComponent(typeof(Curve.TubeGenerator), typeof(Tubular.TubeSettings))]
public class GrindBar : MonoBehaviour
{
    [SerializeField] private Curve.TubeGenerator grindBar = null;
    
    private void Start()
    {
        grindBar = grindBar ? grindBar : GetComponent<Curve.TubeGenerator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GrindAbility __grindAbility = collision.transform.root.GetComponentInChildren<GrindAbility>();

        if(!__grindAbility) return;

        __grindAbility.AttachToBar(grindBar);
    }
}
