#pragma warning disable CS0618 // Type or member is obsolete (for MixerStates in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using Animancer;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private LinearMixerState.Serializable mixer;

    [Range(0,1)]
    [SerializeField] private float currentSpeed = 0f;

    private void OnValidate()
    {
        Speed = currentSpeed;
    }

    private void OnEnable()
    {
        animancer.Transition(mixer);
    }

    /// <summary>
    /// Set by a <see cref="UnityEngine.Events.UnityEvent"/> on a <see cref="UnityEngine.UI.Slider"/>.
    /// </summary>
    public float Speed
    {
        get => mixer.State.Parameter;
        set => mixer.State.Parameter = value;
    }
}
