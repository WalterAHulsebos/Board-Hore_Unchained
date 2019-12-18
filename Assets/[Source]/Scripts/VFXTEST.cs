using UnityEngine;

namespace Core
{
    public sealed class VFXTEST : MonoBehaviour
    {
        [SerializeField] private string tag = "Vehicle";
        [SerializeField] private ParticleSystem particleSystem;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(tag))
                return;

            particleSystem.Play();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(tag))
                return;

            particleSystem.Stop();
        }
    }
}