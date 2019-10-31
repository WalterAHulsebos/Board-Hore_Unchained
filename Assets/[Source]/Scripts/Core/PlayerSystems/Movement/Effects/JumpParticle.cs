using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    public class JumpParticle : VehicleBehaviour
    {
        public ParticleSystem jumpDustParticles;
        public float currentDeadTime;

        private VehicleCore _vehicle;
        
        protected void Start()
        {
            base.Start();
            //_vehicle.OnLanding += PlayJumpParticles;
        }
        
        private void Update()
        {
            if (currentDeadTime > 0)
            {
                currentDeadTime -= Time.deltaTime;
            }
        }


        public void PlayJumpParticles()
        {
            if (!(currentDeadTime <= 0)) return;
            
            jumpDustParticles.transform.localRotation = Quaternion.Euler(_vehicle.averageColliderSurfaceNormal) * Quaternion.Euler(90,0,0);
            jumpDustParticles.Play();
            
            currentDeadTime += 1f;
        }
    }
}