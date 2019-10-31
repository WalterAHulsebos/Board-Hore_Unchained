using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    public class CollisionParticle : VehicleBehaviour
    {
        public ParticleSystem collisionDustParticles;
        public float currentDeadTime = 1f;

        //private ParticleSystem.MainModule _mainModule;
        //private ParticleSystem.EmissionModule _emissionModule;

        private void Awake()
        { 
            //_mainModule = collisionDustParticles.main;
            //_emissionModule = collisionDustParticles.emission;
        }

        protected override void Start()
        {
            ColliderData.EOnCollision += PlayCollisionParticles;
        }

        private void Update()
        {
            if (currentDeadTime > 0)
            {
                currentDeadTime -= Time.deltaTime;
            }
        }
        
        public void PlayCollisionParticles(Collision collision)
        {
            if (!(currentDeadTime <= 0)) return;
            
            float __impulseMag = collision.impulse.magnitude;
            ParticleSystem.MainModule __mainModule = collisionDustParticles.main;
            __mainModule.startSpeed = 5 + __impulseMag / 5;

            __mainModule.startSize = Mathf.Clamp01(__impulseMag / 20);

            ParticleSystem.EmissionModule __emissionModule = collisionDustParticles.emission;
            ParticleSystem.Burst __burst = __emissionModule.GetBurst(0);
            __burst.count = 10 + (int) __impulseMag / 5;

            collisionDustParticles.transform.position = collision.contacts[0].point;
            collisionDustParticles.transform.rotation = Quaternion.Euler(collision.contacts[0].normal);
            collisionDustParticles.Play();
            currentDeadTime += 0.05f;
        }
    }
}