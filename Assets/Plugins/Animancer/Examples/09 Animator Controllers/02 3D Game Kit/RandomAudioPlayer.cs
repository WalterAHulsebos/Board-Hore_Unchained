// Animancer // NOT Copyright 2019 Kybernetik //
// This script was copied from the 3D Game Kit with a few modifications.

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// A copy of the class with the same name in the 3D Game Kit. We can't just use that class because the Animancer
    /// assembly can't reference the 3D Game Kit assembly (because it isn't required to use the rest of Animancer).
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Animancer/Examples/Game Kit - Random Audio Player")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/RandomAudioPlayer")]
    public sealed class RandomAudioPlayer : MonoBehaviour
    {
        [Serializable]
        public sealed class MaterialAudioOverride
        {
            public Material[] materials;
            public SoundBank[] banks;
        }

        [Serializable]
        public sealed class SoundBank
        {
            public string name;
            public AudioClip[] clips;
        }

        public bool randomizePitch = true;
        public float pitchRandomRange = 0.2f;
        public float playDelay = 0;
        public SoundBank defaultBank = new SoundBank();
        public MaterialAudioOverride[] overrides;

        // These fields should not be here because they are only used by the footstep sounds and nothing else so they
        // have been moved to LocomotionState.
        // Also, they should have been [NonSerialized] instead of [HideInInspector] to prevent them from actually
        // getting saved in the scene. Otherwise a script could set the value in the Unity Editor and you would never
        // know why it was in the wrong state on startup.
        //[HideInInspector]
        //public bool playing;
        //[HideInInspector]
        //public bool canPlay;

        // This wasn't readonly, but it should be because we never want to re-assign it after it is created.
        private readonly Dictionary<Material, SoundBank[]>
            Lookup = new Dictionary<Material, SoundBank[]>();

        // This was a protected field with a public property for no reason even though the Clip was an auto-property.
        // It was also called "audioSource" even though the AudioClip was only called "clip". Use consistent naming.
        public AudioSource Source { get; private set; }
        public AudioClip Clip { get; private set; }

        void Awake()
        {
            Source = GetComponent<AudioSource>();
            for (int i = 0; i < overrides.Length; i++)
            {
                foreach (var material in overrides[i].materials)
                    Lookup[material] = overrides[i].banks;
            }
        }

        /// <summary>
        /// Will pick a random clip to play in the assigned list. If you pass a material, it will try to find an
        /// override for that materials or play the default clip if none can ben found.
        /// </summary>
        /// <param name="overrideMaterial"></param>
        /// <returns> Return the choosen audio clip, null if none </returns>
        public AudioClip PlayRandomClip(Material overrideMaterial, int bankId = 0)
        {
#if UNITY_EDITOR
            //UnityEditor.EditorGUIUtility.PingObject(overrideMaterial);
#endif
            if (overrideMaterial == null) return null;
            return InternalPlayRandomClip(overrideMaterial, bankId);
        }

        /// <summary>
        /// Will pick a random clip to play in the assigned list.
        /// </summary>
        public void PlayRandomClip()
        {
            Clip = InternalPlayRandomClip(null, bankId: 0);
        }

        AudioClip InternalPlayRandomClip(Material overrideMaterial, int bankId)
        {
            SoundBank[] banks;
            var bank = defaultBank;
            if (overrideMaterial != null)
                if (Lookup.TryGetValue(overrideMaterial, out banks))
                    if (bankId < banks.Length)
                        bank = banks[bankId];
            if (bank.clips == null || bank.clips.Length == 0)
                return null;
            var clip = bank.clips[Random.Range(0, bank.clips.Length)];

            if (clip == null)
                return null;

            Source.pitch = randomizePitch ? Random.Range(1.0f - pitchRandomRange, 1.0f + pitchRandomRange) : 1.0f;
            Source.clip = clip;
            Source.PlayDelayed(playDelay);

            return clip;
        }

    }
}
