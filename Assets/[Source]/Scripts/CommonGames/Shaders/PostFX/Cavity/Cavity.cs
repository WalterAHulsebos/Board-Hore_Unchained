using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor.Rendering.PostProcessing;
#endif

[Serializable]
[PostProcess(typeof(CavityRenderer), PostProcessEvent.AfterStack, "CommonGames/Cavity")]
public sealed class Cavity : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Cavity effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class CavityRenderer : PostProcessEffectRenderer<Cavity>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Cavity"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

#if UNITY_EDITOR

[PostProcessEditor(typeof(Cavity))]
public sealed class CavityEditor : PostProcessEffectEditor<Cavity>
{
    SerializedParameterOverride m_Blend;

    public override void OnEnable()
    {
        m_Blend = FindParameterOverride(x => x.blend);
    }

    public override void OnInspectorGUI()
    {
        PropertyField(m_Blend);
    }
}

#endif